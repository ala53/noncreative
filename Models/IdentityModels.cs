using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NonCreative.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string DisplayName { get; set; }
        public string TokenKeyPublic { get; set; }
        public string TokenKeyPrivate { get; set; }
        public virtual ICollection<UserWallReference> OwnedWalls { get; set; }
        public virtual ICollection<UserPostReference> OwnedPosts { get; set; }
        public virtual ICollection<UserWallReference> AuthorizedWalls { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
        private UserWallReference Find(WallModel model)
        {
            foreach (var wall in OwnedWalls)
                if (wall.WallId == model.WallUrl)
                    return wall;

            return null;
        }
        public bool Contains(WallModel wall)
        {
            return Find(wall) != null;
        }
        public void Add(WallModel wall)
        {
            var model = DatabaseContext.Shared.WallReferences.Create();
            model.WallId = wall.WallUrl;
            model.Wall = wall;
            OwnedWalls.Add(model);
        }
        public void Remove(WallModel wall)
        {
            if (Contains(wall))
                OwnedWalls.Remove(Find(wall));
        }
        private UserPostReference Find(WallPost model)
        {
            foreach (var wall in OwnedPosts)
                if (wall.PostId == model.PostId)
                    return wall;

            return null;
        }
        public bool Contains(WallPost post)
        {
            return Find(post) != null;
        }
        public void Add(WallPost post)
        {
            var model = DatabaseContext.Shared.PostReferences.Create();
            model.PostId = post.PostId;
            model.Post = post;
            OwnedPosts.Add(model);
        }
        public void Remove(WallPost post)
        {
            if (Contains(post))
                OwnedPosts.Remove(Find(post));
        }
    }
}