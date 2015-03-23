using NonCreative.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace NonCreative.Controllers
{
    public partial class WallController : ApiController
    {
        private string GetPrivateKey(GenericWallRequest model)
        {
            var user = User.Identity.IsAuthenticated ? _authRepo.FindUserByNameSync(User.Identity.Name) : null;
            if (user != null) return _authRepo.FindUserByNameSync(User.Identity.Name).TokenKeyPrivate;
            else return model.KeyPrivate;
        }
        private string GetPassword(GenericWallRequest model)
        {
            return model.Password;
        }
        private string GetPublicKey(GenericWallRequest model)
        {
            var user = User.Identity.IsAuthenticated ? _authRepo.FindUserByNameSync(User.Identity.Name) : null;
            if (user != null) return _authRepo.FindUserByNameSync(User.Identity.Name).TokenKeyPublic;
            else return model.KeyPublic;
        }

        private bool IsOwner(WallModel model, GenericWallRequest request)
        {
            return model.OwnerPrivate == GetPrivateKey(request);
        }
        private bool CanEdit(WallModel model, WallPost post, dynamic request)
        {
            //No view permission
            if (!IsAuthorizedToView(model, GetPassword(request))) return false;
            //Guaranteed to have edit permission if can moderate
            if (CanModerate(model, request)) return true;
            //Check that create permission is enabled and that user is owner
            return CanCreatePost(model, request) && post.CreatorPrivate == GetPrivateKey(request);
        }

        private bool CanCreatePost(WallModel model, GenericWallRequest request)
        {
            //No view permission
            if (!IsAuthorizedToView(model, GetPassword(request))) return false;
            if (CanModerate(model, request)) return true;
            if (IsEditPermission(model.UnauthorizedUserPermissionLevel)) //If anyone can edit
                return true;

            return UserHasCreatePermissions(model);
        }

        private bool CanModerate(WallModel model, GenericWallRequest request)
        {
            //No view permission
            if (!IsAuthorizedToView(model, GetPassword(request))) return false;
            //Everyone has admin
            if (model.UnauthorizedUserPermissionLevel ==
                WallModel.WallAccessPermissionLevels.ViewEditModerate) return true;
            //Owner
            if (IsOwner(model, request)) return true;
            //Not authorized to do anything / aren't on authorized list
            if (!User.Identity.IsAuthenticated) return false;
            //Get them if possible
            var mx = model.AuthorizedUsers.FirstOrDefault(
                (m) => m.User.UserName == User.Identity.Name &&
                m.PermissionLevel == WallModel.WallAccessPermissionLevels.ViewEditModerate);
            //and verify
            return mx != null;
        }

        private bool UserHasCreatePermissions(WallModel model)
        {
            if (!User.Identity.IsAuthenticated) return false;
            return model.AuthorizedUsers.FirstOrDefault((m) => m.User.UserName == User.Identity.Name &&
                m.PermissionLevel == WallModel.WallAccessPermissionLevels.ViewEdit ||
                m.PermissionLevel == WallModel.WallAccessPermissionLevels.ViewEditModerate) != null;
        }

        private string GetUsername()
        {
            if (User.Identity.IsAuthenticated)
            {
                var usr = _authRepo.FindUserByNameSync(User.Identity.Name);
                return usr.DisplayName;
            }
            else return "Anonymous";
        }

        /// <summary>
        /// Checks that view permissions are given.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        private bool IsAuthorizedToView(WallModel model, string pass)
        {
            if (model.Password == null || model.Password == "" &&
                IsViewPermission(model.UnauthorizedUserPermissionLevel)) return true;

            if (model.Password == pass && IsViewPermission(model.UnauthorizedUserPermissionLevel)) return true;

            var user = User.Identity.IsAuthenticated ? _authRepo.FindUserByNameSync(User.Identity.Name) : null;

            return (user != null ? user.TokenKeyPrivate == model.OwnerPrivate : false) ||
                (user != null ? model.AuthorizedUsers.FirstOrDefault((m) =>
                    (m.PermissionLevel == WallModel.WallAccessPermissionLevels.View ||
                    m.PermissionLevel == WallModel.WallAccessPermissionLevels.ViewEdit ||
                    m.PermissionLevel == WallModel.WallAccessPermissionLevels.ViewEditModerate) &&
                    m.User.UserName == User.Identity.Name) != null : false);
        }

        private bool IsViewPermission(WallModel.WallAccessPermissionLevels lvl)
        {
            return lvl == WallModel.WallAccessPermissionLevels.View ||
                lvl == WallModel.WallAccessPermissionLevels.ViewEdit ||
                lvl == WallModel.WallAccessPermissionLevels.ViewEditModerate;
        }
        private bool IsEditPermission(WallModel.WallAccessPermissionLevels lvl)
        {
            return lvl == WallModel.WallAccessPermissionLevels.ViewEdit ||
                lvl == WallModel.WallAccessPermissionLevels.ViewEditModerate;
        }

        private ApplicationUser GetUser()
        {
            if (!User.Identity.IsAuthenticated) return null;
            return _authRepo.FindUserByNameSync(User.Identity.Name);
        }

    }
}