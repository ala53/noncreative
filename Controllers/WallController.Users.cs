using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using NonCreative.Models;

namespace NonCreative.Controllers
{
    public partial class WallController : ApiController
    {
        [HttpPost, Route("users/authorizedUser/remove/{wallId}")]
        public IHttpActionResult RemoveAuthorizedUser(string wallId, RemoveAuthorizedUserRequestModel request)
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

            model.RemoveAuthorizedUser(request.Username);
            var info = GetInfo(model);
            try
            {
                DatabaseContext.Shared.SaveChanges();
                DatabaseContext.Release();
                return Ok(info);
            }
            catch
            {
                DatabaseContext.Release();
                return NotFound();
            }

        }
        [HttpPost, Route("users/authorizedUser/add/{wallId}")]
        public IHttpActionResult AddAuthorizedUser(string wallId, AddOrUpdateAuthorizedUserRequestModel request)
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

            var user = _authRepo.FindUserByNameSync(request.Username);
            if (user == null)
                return NotFound();

            model.AddAuthorizedUser(user, request.PermissionLevel);
            var info = GetInfo(model);
            try
            {
                DatabaseContext.Shared.SaveChanges();
                DatabaseContext.Release();
                return Ok(info);
            }
            catch
            {
                DatabaseContext.Release();
                return InternalServerError();
            }
        }
        [HttpPost, Route("users/authorizedUser/update/{wallId}")]
        public IHttpActionResult UpdateAuthorizedUser(string wallId, AddOrUpdateAuthorizedUserRequestModel request)
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

            var user = _authRepo.FindUserByNameSync(request.Username);
            if (user == null)
                return NotFound();

            model.AddAuthorizedUser(user, request.PermissionLevel);
            var info = GetInfo(model);
            try
            {
                DatabaseContext.Shared.SaveChanges();
                DatabaseContext.Release();
                return Ok(info);
            }
            catch
            {
                DatabaseContext.Release();
                return InternalServerError();
            }
        }
    }
}