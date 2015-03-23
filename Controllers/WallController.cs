using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using NonCreative.Models;
using System.Data.Entity.Validation;
using System.Web;
using System.Text.RegularExpressions;

namespace NonCreative.Controllers
{
    public partial class WallController : ApiController
    {
        private AuthSystem.AuthRepository _authRepo = new AuthSystem.AuthRepository();

        // POST: api/Walls/create
        [ResponseType(typeof(WallModelInfoResponse)), HttpPost, Route("create")]
        public IHttpActionResult CreateWall(WallModelCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var ownerName = GetUsername();

            var wallModel = DatabaseContext.Shared.WallModels.Create();

            wallModel.OwnerName = ownerName;
            wallModel.OwnerPrivate = GetPrivateKey(request);
            wallModel.OwnerPublic = GetPublicKey(request);
            wallModel.Password = request.WantedPassword;
            wallModel.WallUrl = Helpers.TextSanitizer.Hypersanitize(request.RequestedUrl, true);
            wallModel.Title = Helpers.TextSanitizer.MakeSafe(request.Title, false);
            wallModel.Subtitle = Helpers.TextSanitizer.MakeSafe(request.Subtitle, false);
            wallModel.BackgroundUrl = Helpers.TextSanitizer.MakeSafe(request.BackgroundUrl, false);
            if (request.TileBackground != null)
                wallModel.TileBackground = request.TileBackground.Value;
            wallModel.WallMode = (request.WallMode == WallModel.WallModes.INVALID ?
                WallModel.WallModes.Stream : request.WallMode);
            wallModel.UnauthorizedUserPermissionLevel =
                (request.UnauthorizedUserPermissions == WallModel.WallAccessPermissionLevels.INVALID ?
                WallModel.WallAccessPermissionLevels.ViewEdit : request.UnauthorizedUserPermissions);

            var post = GetStarterPost();
            wallModel.AddPost(post);

            DatabaseContext.Shared.WallModels.Add(wallModel);

            try
            {
                DatabaseContext.Shared.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                if (WallModelExists(wallModel.WallUrl))
                {
                    return Conflict();
                }
                else
                {
                    throw e;
                }
            }
            //Add to user, if any
            var user = GetUser();
            if (user != null)
            {
                user.Add(wallModel);
                _authRepo.SaveUserUpdate();
            }
            var info = GetInfo(wallModel);
            DatabaseContext.Release();
            return Ok(info);
        }

        //GET /api/walls/info/{wallId}
        [HttpGet, Route("info/{wallId}")]
        public IHttpActionResult GetWallInfo(string wallId)
        {
            wallId = Helpers.TextSanitizer.Hypersanitize(wallId, true);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = DatabaseContext.Shared.WallModels.Find(wallId);
            if (model == null)
                return NotFound();
            var info = GetInfo(model);
            DatabaseContext.Release();
            return Ok(info);
        }

        private WallModelInfoResponse GetInfo(WallModel model)
        {
            var users = new List<WallModelInfoAuthorizedUsersResponse>();
            foreach (var usr in model.AuthorizedUsers)
                users.Add(new WallModelInfoAuthorizedUsersResponse()
                {
                    KeyPublic = usr.User.TokenKeyPublic,
                    Permissions = usr.PermissionLevel,
                    Username = usr.User.UserName
                });

            return new WallModelInfoResponse()
            {
                OwnerName = model.OwnerName,
                OwnerPublic = model.OwnerPublic,
                HasPassword = model.Password != null && model.Password != "",
                Title = model.Title,
                Subtitle = model.Subtitle,
                Url = model.WallUrl,
                AuthorizedUsers = users,
                BackgroundUrl = model.BackgroundUrl,
                TileBackground = model.TileBackground,
                IsPrivate = !IsViewPermission(model.UnauthorizedUserPermissionLevel),
                UnauthorizedUsersPermission = model.UnauthorizedUserPermissionLevel,
                WallMode = model.WallMode,
            };
        }

        // POST: api/walls/delete/{wallId}
        [HttpPost, Route("delete/{wallId}")]
        public IHttpActionResult DeleteWall(string wallId, GenericWallRequest request)
        {
            wallId = Helpers.TextSanitizer.Hypersanitize(wallId, true);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = DatabaseContext.Shared.WallModels.Find(wallId);
            if (model == null)
                return NotFound();

            if (!IsOwner(model, request))
                return Unauthorized();

            //Delete posts first to avoid reference errors
            DatabaseContext.Shared.WallPosts.RemoveRange(model.Posts);

            //And delete the files (both references and actual files)
            foreach (var file in model.Files)
                TryDeleteFile(file); //Remove actual files

            DatabaseContext.Shared.Files.RemoveRange(model.Files); //Remove db info

            //And remove the wall from the user, if any
            var user = GetUser();
            if (user != null)
            {
                user.Remove(model);
                _authRepo.SaveUserUpdate();
            }

            DatabaseContext.Shared.WallModels.Remove(model);

            try
            {
                DatabaseContext.Shared.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (WallModelExists(wallId))
                {
                    return InternalServerError();
                }
                else
                {
                    throw;
                }
            }

            DatabaseContext.Release();
            return Ok();
        }

        [HttpGet, Route("exists/{wallId}")]
        public IHttpActionResult CheckWallExists(string wallId)
        {
            wallId = Helpers.TextSanitizer.Hypersanitize(wallId, true);

            var wallModel = DatabaseContext.Shared.WallModels.Find(wallId);
            if (wallModel == null)
                return Ok(new { exists = false });

            return Ok(new { exists = true });
        }


        // POST: api/walls/update/{wallId}
        [HttpPost, Route("update/{wallId}")]
        public IHttpActionResult UpdateWall(string wallId, WallModelCreateRequest request)
        {
            wallId = Helpers.TextSanitizer.Hypersanitize(wallId, true);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var wallModel = DatabaseContext.Shared.WallModels.Find(wallId);
            if (wallModel == null)
                return NotFound();

            if (!IsOwner(wallModel, request))
                return Unauthorized();

            if (request.Password != null)
                wallModel.Password = request.WantedPassword;
            if (request.BackgroundUrl != null)
                wallModel.BackgroundUrl = Helpers.TextSanitizer.MakeSafe(request.BackgroundUrl, false);
            if (request.TileBackground != null)
                wallModel.TileBackground = request.TileBackground.Value;
            if (request.Title != null)
                wallModel.Title = Helpers.TextSanitizer.MakeSafe(request.Title, false);
            if (request.Subtitle != null)
                wallModel.Subtitle = Helpers.TextSanitizer.MakeSafe(request.Subtitle, false);
            if (request.WallMode != WallModel.WallModes.INVALID)
                wallModel.WallMode = request.WallMode;
            if (request.UnauthorizedUserPermissions != WallModel.WallAccessPermissionLevels.INVALID)
                wallModel.UnauthorizedUserPermissionLevel = request.UnauthorizedUserPermissions;

            DatabaseContext.Shared.SaveChanges();
            var info = GetInfo(wallModel);
            DatabaseContext.Release();
            return Ok(info);
        }

        [HttpPost, Route("checkpassword/{wallId}")]
        public IHttpActionResult CheckPassword(string wallId, GenericWallRequest request)
        {
            wallId = Helpers.TextSanitizer.Hypersanitize(wallId, true);
            if (!ModelState.IsValid)
                return BadRequest();

            WallModel wallModel = DatabaseContext.Shared.WallModels.Find(wallId);

            if (wallModel == null)
            {
                return NotFound();
            }

            DatabaseContext.Release();
            return Ok(new { authenticated = request.Password == wallModel.Password });
        }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private bool WallModelExists(string id)
        {
            return DatabaseContext.Shared.WallModels.Count(e => e.WallUrl == id) > 0;
        }
    }
}