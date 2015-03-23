using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using NonCreative.Models;
using System.Data.Entity.Infrastructure;

namespace NonCreative.Controllers
{
    public partial class WallController : ApiController
    {
        [HttpPost, Route("import/padlet/{wallId}")]
        public IHttpActionResult ImportFromPadlet(string wallId, PadletImportRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            wallId = Helpers.TextSanitizer.Hypersanitize(wallId, true);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = DatabaseContext.Shared.WallModels.Find(wallId);
            if (model == null)
                return NotFound();

            if (!CanModerate(model, request))
                return Unauthorized();

            var data = Importers.PadletImporter.ImportFromCsv(request.CSVData);

            foreach (var post in data)
            {
                model.AddPost(post);
                DatabaseContext.Shared.WallPosts.Add(post);
            }

            try
            {
                DatabaseContext.Shared.SaveChanges();
                DatabaseContext.Release();
                return Ok();
            }
            catch (DbUpdateConcurrencyException)
            {
                return InternalServerError();
            }
        }
    }
}