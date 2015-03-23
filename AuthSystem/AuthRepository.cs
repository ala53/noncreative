using NonCreative.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace NonCreative.AuthSystem
{
    public class AuthRepository : IDisposable
    {
        private UserManager<ApplicationUser> _userManager;

        #region Wall helpers

        #endregion

        public AuthRepository()
        {
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(DatabaseContext.Shared));
        }

        public async Task<IdentityResult> RegisterUser(UserModel userModel)
        {
            ApplicationUser user = new ApplicationUser
            {
                UserName = userModel.UserName,
                DisplayName = userModel.DisplayName,
                TokenKeyPrivate = Guid.NewGuid().ToString(),
                TokenKeyPublic = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, userModel.Password);
            Hooks.OnRegistered(FindUserByNameSync(userModel.UserName), this);
            SaveUserUpdate();
            return result;
        }
        public IdentityResult RegisterUserSync(UserModel userModel)
        {
            ApplicationUser user = new ApplicationUser
            {
                UserName = userModel.UserName,
                DisplayName = userModel.DisplayName,
                TokenKeyPrivate = Guid.NewGuid().ToString(),
                TokenKeyPublic = Guid.NewGuid().ToString()
            };

            var result = _userManager.Create(user, userModel.Password);
            Hooks.OnRegistered(FindUserByNameSync(userModel.UserName), this);
            SaveUserUpdate();
            return result;
        }

        public void SaveUserUpdate()
        {
            try
            {
                DatabaseContext.Shared.SaveChanges();
                DatabaseContext.Shared.Dispose();
            }
            catch (Exception ex)
            {

            }
        }

        public async Task<IdentityResult> ChangePassword(UserChangePasswordModel model)
        {
            Hooks.OnPasswordChanged(FindUserByNameSync(model.UserName), this);
            var result = await _userManager.ChangePasswordAsync(
                model.UserName, model.OldPassword, model.NewPassword);
            return result;
        }
        public IdentityResult ChangePasswordSync(UserChangePasswordModel model)
        {
            Hooks.OnPasswordChanged(FindUserByNameSync(model.UserName), this);
            SaveUserUpdate();
            return _userManager.ChangePassword(model.UserName, model.OldPassword, model.NewPassword);
        }

        public async Task<ApplicationUser> FindUser(string userName, string password)
        {
            ApplicationUser user = await _userManager.FindAsync(userName, password);

            return user;
        }
        public ApplicationUser FindUserSync(string userName, string password)
        {
            return _userManager.Find(userName, password);
        }

        public async Task<ApplicationUser> FindUserByName(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }
        public ApplicationUser FindUserByNameSync(string username)
        {
            return _userManager.FindByName(username);
        }

        public void Dispose()
        {
        }
    }
}