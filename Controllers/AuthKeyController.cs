using NonCreative.AuthSystem;
using NonCreative.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NonCreative.Controllers
{
    [RoutePrefix("api/key")]
    public class AuthKeyController : ApiController
    {
        private AuthRepository _authRepo = new AuthRepository();

        //GET: api/key
        [AllowAnonymous, HttpGet, Route("")]
        public AuthKeyModel GetAuthKey()
        {
            var header = Request.Headers;
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var user = _authRepo.FindUserByNameSync(User.Identity.Name);

                if (user.TokenKeyPrivate == null || user.TokenKeyPublic == null)
                {
                    user.TokenKeyPublic = Guid.NewGuid().ToString();
                    user.TokenKeyPrivate = Guid.NewGuid().ToString();
                    _authRepo.SaveUserUpdate();
                }
                //They exist, don't create a new key
                return new AuthKeyModel()
                {
                    KeyPrivate = user.TokenKeyPrivate,
                    KeyPublic = user.TokenKeyPublic
                };
            }
            else
            {
                return new AuthKeyModel()
                {
                    KeyPrivate = Guid.NewGuid().ToString(),
                    KeyPublic = Guid.NewGuid().ToString()
                };
            }
        }
    }
}
