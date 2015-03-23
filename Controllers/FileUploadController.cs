using NonCreative.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace NonCreative.Controllers
{
    [RoutePrefix("api/walls")]
    public partial class WallController : ApiController
    {
        [HttpPost, Route("upload/{wallId}")]
        public IHttpActionResult UploadFile(string wallId, GenericWallRequest _request)
        {
            HttpRequestMessage request = this.Request;
            if (!request.Content.IsMimeMultipartContent())
            {
                return BadRequest();
            }

            if (!WallExists(wallId))
                return NotFound();

            var wall = DatabaseContext.Shared.WallModels.Find(wallId);

            if (!CanCreatePost(wall, _request))
                return Unauthorized();

            wallId = Helpers.TextSanitizer.Hypersanitize(wallId).ToLower();
            if (wallId.Length <= 0)
                return InternalServerError();

            if (wallId.Length >= 40) //Don't allow length >=40 for wall id
                wallId = wallId.Substring(0, 40);
            //Get the directory to store
            string root = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/uploads/" + wallId);
            System.IO.Directory.CreateDirectory(root);

            //And loop through and get all of the files
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                var locations = new List<FileLocation>();
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    //get the filename to upload to
                    var extension = System.IO.Path.GetExtension(postedFile.FileName);
                    var fileName = GetFileName(36) + "." + extension;
                    var savePath = System.IO.Path.Combine(root, fileName);
                    //In case we generate identical names, loop until they're different
                    while (System.IO.File.Exists(savePath))
                    {
                        fileName = GetFileName(36) + "." + extension;
                        savePath = System.IO.Path.Combine(root, fileName);
                    }
                    //And save
                    postedFile.SaveAs(savePath);
                    locations.Add(new FileLocation()
                    {
                        Filename = fileName,
                        AbsolutePath = String.Format("/api/walls/retrieve/{0}/{1}", wallId, fileName)
                    });
                }
                //And add to the wall db
                foreach (var fl in locations)
                {
                    var fum = DatabaseContext.Shared.Files.Create();
                    fum.Filename = fl.Filename;
                    fum.Wall = wall;
                    fum.WallId = wall.WallUrl;
                    wall.Files.Add(fum);
                }
                DatabaseContext.Shared.SaveChanges();
                //And return the info
                return Ok(locations);
            }
            else
            {
                return BadRequest("No files to upload.");
            }
            DatabaseContext.Release();
        }

        //GET /api/walls/retrieve/{wallid}/{filename}
        [HttpPost, Route("retrieve/{wallId}/{filename}")]
        public IHttpActionResult GetUploadedFile(string wallId, string filename, GenericWallRequest request)
        {
            var sanitized = Helpers.TextSanitizer.Hypersanitize(wallId).ToLower();
            if (sanitized.Length <= 0)
                return InternalServerError();

            if (!WallExists(wallId))
                return NotFound();

            var wall = DatabaseContext.Shared.WallModels.Find(wallId);

            if (!CanCreatePost(wall, request))
                return Unauthorized();

            wallId = Helpers.TextSanitizer.Hypersanitize(wallId).ToLower();
            if (wallId.Length <= 0)
                return InternalServerError();

            if (sanitized.Length >= 40) //Don't allow length >=40 for wall id
                sanitized = sanitized.Substring(0, 40);
            //Get the directory to store
            string root = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/uploads/" + sanitized);
            System.IO.Directory.CreateDirectory(root);

            //Sanitize the filename
            filename = Helpers.TextSanitizer.Hypersanitize(filename, false, true);

            if (!System.IO.File.Exists(System.IO.Path.Combine(root, filename)))
                return NotFound();
            try
            {
                var stream = new System.IO.FileStream(System.IO.Path.Combine(root, filename),
                    System.IO.FileMode.Open, System.IO.FileAccess.Read);

                var content = new StreamContent(stream);
                var mimeType = MimeTypeMap.MimeTypeMap.GetMimeType(System.IO.Path.GetExtension(filename));

                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = content;
                result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mimeType);
                return Ok(result);
            }
            catch
            {
                return NotFound();
            }
        }

        public static void TryDeleteFile(Models.FileUploadModel _file)
        {
            var sanitized = Helpers.TextSanitizer.Hypersanitize(_file.WallId).ToLower();
            if (sanitized.Length <= 0)
                return;

            if (sanitized.Length >= 40) //Don't allow length >=40 for wall id
                sanitized = sanitized.Substring(0, 40);

            string root = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/uploads/" + sanitized);
            if (System.IO.File.Exists(System.IO.Path.Combine(root, _file.Filename)))
            {
                System.IO.File.Delete(System.IO.Path.Combine(root, _file.Filename));
            }

        }

        private Random _rand;
        private string GetFileName(int length)
        {
            var allowed = "abcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
            var output = "";
            for (var i = 0; i < length; i++)
                output += allowed[_rand.Next(0, allowed.Length - 1)];

            return output;
        }

        private bool WallExists(string wallName)
        {
            return DatabaseContext.Shared.WallModels.Count((e) => e.WallUrl == wallName) > 0;
        }

        public class FileLocation
        {
            public string AbsolutePath { get; set; }
            public string Filename { get; set; }
        }
    }
}