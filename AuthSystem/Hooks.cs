using NonCreative.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NonCreative.AuthSystem
{
    public static class Hooks
    {
        public static void OnPasswordChanged(ApplicationUser user, AuthRepository repo)
        {
            user.TokenKeyPrivate = Guid.NewGuid().ToString();
            user.TokenKeyPublic = Guid.NewGuid().ToString();
            UpdatePostsAndWalls(user);
        }
        public static void OnRegistered(ApplicationUser user, AuthRepository repo)
        {
            user.TokenKeyPrivate = Guid.NewGuid().ToString();
            user.TokenKeyPublic = Guid.NewGuid().ToString();
        }

        private static void UpdatePostsAndWalls(ApplicationUser user)
        {
            foreach (var wallRef in user.OwnedWalls)
            {
                var wall = wallRef.Wall;
                wall.OwnerPublic = user.TokenKeyPublic;
                wall.OwnerPrivate = user.TokenKeyPrivate;
            }
            foreach (var postRef in user.OwnedPosts)
            {
                var post = postRef.Post;
                post.CreatorPublic = user.TokenKeyPublic;
                post.CreatorPrivate = user.TokenKeyPrivate;
            }
            DatabaseContext.Shared.SaveChanges();
        }
    }
}