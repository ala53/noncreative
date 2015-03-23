using NonCreative.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace NonCreative.Controllers
{
    public partial class WallController
    {
        private WallModelPostInfoResponse GetPostInfo(WallPost model)
        {
            return new WallModelPostInfoResponse()
            {
                Header = model.Header ?? "",
                Content = model.Content ?? "",
                KeyPublic = model.CreatorPublic,
                PostId = model.PostId,
                CreationTime = model.CreationTime,
                Author = model.CreatorName,
                Attachment = model.Attachment,
                SortOrder = model.SortOrder,
                UpdateTime = model.UpdateTime,
                X = model.XPosition,
                Y = model.YPosition
            };
        }

        private WallPost GetStarterPost()
        {
            var post = DatabaseContext.Shared.WallPosts.Create();
            post.Header = "A warm welcome from the admin...";
            post.Content = @"Here's your new and fancy <strike>page</strike> <b>wall</b>. HTML is supported, though
                <br/>only on a whitelisted basis so bad people cannot do any damage. Don't worry - this page is safe. 
                <br/><blockquote>(Just click the delete button to get rid of me or click <a href='#/help' target='_blank'>here</a> for more info.)</blockquote>";
            post.CreationTime = DateTime.UtcNow;
            post.CreatorName = "The Wonderful Admin";
            post.CreatorPrivate = Guid.NewGuid().ToString();
            post.CreatorPublic = Guid.NewGuid().ToString();
            post.UpdateTime = DateTime.UtcNow;
            post.XPosition = 100;
            post.YPosition = 100;
            return post;
        }

        //POST api/walls/addpost/{wallId}
        [HttpPost, Route("addpost/{wallId}")]
        public IHttpActionResult AddPost(string wallId, WallModelAddPostRequest request)
        {
            wallId = Helpers.TextSanitizer.Hypersanitize(wallId, true);
            if (!ModelState.IsValid)
                return BadRequest();


            WallModel wallModel = DatabaseContext.Shared.WallModels.Find(wallId);

            if (wallModel == null)
            {
                return NotFound();
            }
            if (!CanCreatePost(wallModel, request))
                return Unauthorized();

            var username = GetUsername();

            var post = DatabaseContext.Shared.WallPosts.Create();
            post.Attachment = Helpers.TextSanitizer.MakeSafe(request.AttachmentUrl, false);
            post.Content = Helpers.TextSanitizer.MakeSafe(request.Content, true);
            post.CreationTime = DateTime.UtcNow;
            post.UpdateTime = DateTime.UtcNow;
            post.CreatorName = username;
            post.CreatorPrivate = GetPrivateKey(request);
            post.CreatorPublic = GetPublicKey(request);
            post.Header = Helpers.TextSanitizer.MakeSafe(request.Header, false) ?? "";
            post.Wall = wallModel;
            post.WallId = wallModel.WallUrl;

            wallModel.AddPost(post);

            try
            {
                DatabaseContext.Shared.SaveChanges();
            }
            catch (Exception e)
            {
                return InternalServerError();
            }

            //Add to the user if any
            var user = GetUser();
            if (user != null)
            {
                user.Add(post);
                _authRepo.SaveUserUpdate();
            }

            DatabaseContext.Release();
            return Ok(GetPostInfo(post));
        }


        //POST api/walls/deletepost/{wallId}
        [HttpPost, Route("deletepost/{wallId}")]
        public IHttpActionResult DeletePost(string wallId, WallModelDeletePostRequest request)
        {
            wallId = Helpers.TextSanitizer.Hypersanitize(wallId, true);
            if (!ModelState.IsValid)
                return BadRequest();


            WallModel wallModel = DatabaseContext.Shared.WallModels.Find(wallId);

            if (wallModel == null)
            {
                return NotFound();
            }
            var post = wallModel.Posts.Where((w) => w.PostId == request.PostId).FirstOrDefault();

            if (post == null)
                return NotFound();

            if (!CanEdit(wallModel, post, request))
                return Unauthorized();

            wallModel.RemovePost(post);
            DatabaseContext.Shared.WallPosts.Remove(post);
            //Remove post
            var user = GetUser();
            if (user != null)
            {
                user.Remove(post);
                _authRepo.SaveUserUpdate();
            }

            try
            {
                DatabaseContext.Shared.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return InternalServerError();
            }

            DatabaseContext.Release();
            return Ok();
        }
        //POST api/walls/editpost/{wallId}
        [HttpPost, Route("editpost/{wallId}")]
        public IHttpActionResult EditPost(string wallId, WallModelEditPostRequest request)
        {
            wallId = Helpers.TextSanitizer.Hypersanitize(wallId, true);
            if (!ModelState.IsValid)
                return BadRequest();


            WallModel wallModel = DatabaseContext.Shared.WallModels.Find(wallId);

            if (wallModel == null)
            {
                return NotFound();
            }
            var post = wallModel.Posts.Where((w) => w.PostId == request.PostId).FirstOrDefault();

            if (post == null)
                return NotFound();

            if (!CanEdit(wallModel, post, request))
                return Unauthorized();

            post.Header = (request.NewHeader == null ? post.Header : Helpers.TextSanitizer.MakeSafe(request.NewHeader, false));
            post.Content = (request.NewContent == null || request.NewContent == "" ?
                post.Content : Helpers.TextSanitizer.MakeSafe(request.NewContent, true));
            post.Attachment = (request.NewAttachment == null ? post.Attachment :
                Helpers.TextSanitizer.MakeSafe(request.NewAttachment, false));
            post.XPosition = (request.NewX == 0 ? post.XPosition : request.NewX);
            post.YPosition = (request.NewY == 0 ? post.YPosition : request.NewY);
            post.UpdateTime = DateTime.UtcNow;

            try
            {
                DatabaseContext.Shared.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return InternalServerError();
            }

            DatabaseContext.Release();
            return Ok(GetPostInfo(post));
        }
        // POST: api/walls/posts/{wall id}
        [ResponseType(typeof(IEnumerable<WallPost>)), HttpPost, Route("posts/{wallId}")]
        public IHttpActionResult GetPosts(string wallId, Models.WallChunkRequest chunks)
        {
            wallId = Helpers.TextSanitizer.Hypersanitize(wallId, true);
            if (!ModelState.IsValid)
                return BadRequest();

            WallModel wallModel = DatabaseContext.Shared.WallModels.Find(wallId);
            if (wallModel == null)
            {
                return NotFound();
            }

            var headers = HttpContext.Current.Request.Headers;

            if (!IsAuthorizedToView(wallModel, chunks.Password))
                return Unauthorized();

            if (chunks.Count <= 0 || chunks.Count > wallModel.Posts.Count)
                chunks.Count = wallModel.Posts.Count;

            //We do some spinning around so that the newest is #0 and oldest is X, rather than the reverse
            //That way, a call to (0) yields the newest results and not the oldest
            var relativeStart = wallModel.Posts.Count - chunks.Beginning - chunks.Count;

            //If we are requesting past the end, return empty rather than the entire list
            if (relativeStart < 0) return Ok(Enumerable.Empty<WallModelPostInfoResponse>());

            var posts = new List<WallModelPostInfoResponse>();
            foreach (var post in wallModel.Posts.Skip(relativeStart).Take(chunks.Count))
                posts.Add(GetPostInfo(post));

            //reverse posts so newest are first
            posts.Reverse();

            DatabaseContext.Release();
            return Ok(posts);
        }
    }
}