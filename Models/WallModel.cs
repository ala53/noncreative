using NonCreative.Controllers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace NonCreative.Models
{
    public class WallModel
    {
        [System.ComponentModel.DataAnnotations.Key, Required]
        public string WallUrl { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Subtitle { get; set; }
        [XmlIgnore, Required]
        public string OwnerPrivate { get; set; }
        [Required]
        public string OwnerPublic { get; set; }
        [Required]
        public string OwnerName { get; set; }
        public WallModes WallMode { get; set; }
        public WallAccessPermissionLevels UnauthorizedUserPermissionLevel { get; set; }
        public string BackgroundUrl { get; set; }
        public bool TileBackground { get; set; }

        private ICollection<WallUserReferenceModel> _authorizedUsers;
        public virtual ICollection<WallUserReferenceModel> AuthorizedUsers
        {
            get { return _authorizedUsers ?? (_authorizedUsers = new Collection<WallUserReferenceModel>()); }
            set
            {
                _authorizedUsers = value;
            }
        }
        public string Password { get; set; }
        private ICollection<WallPost> _posts;
        public virtual ICollection<WallPost> Posts
        {
            get { return _posts ?? (_posts = new Collection<WallPost>()); }
            protected set
            {
                _posts = value;
            }
        }

        private ICollection<FileUploadModel> _files;
        public virtual ICollection<FileUploadModel> Files
        {
            get { return _files ?? (_files = new Collection<FileUploadModel>()); }
            protected set
            {
                _files = value;
            }
        }

        public void AddAuthorizedUser(ApplicationUser user, WallAccessPermissionLevels level)
        {
            var usr = AuthorizedUsers.FirstOrDefault((m) => m.User.UserName == user.UserName);
            if (usr != null)
                usr.PermissionLevel = level;
            else
                AuthorizedUsers.Add(new WallUserReferenceModel()
                {
                    PermissionLevel = level,
                    User = user
                });
        }

        public void RemoveAuthorizedUser(string name)
        {
            var usr = AuthorizedUsers.FirstOrDefault((m) => m.User.UserName == name);
            if (usr != null)
                AuthorizedUsers.Remove(usr);
        }

        public void AddPost(WallPost post)
        {
            post.Wall = this;
            post.WallId = this.WallUrl;
            Posts.Add(post);
        }

        public void RemovePost(WallPost post)
        {
            Posts.Remove(post);
            post.Wall = null;
            post.WallId = null;
        }
        public enum WallAccessPermissionLevels
        {
            INVALID = 0,
            None = 1,
            View = 2,
            ViewEdit = 3,
            ViewEditModerate = 4
        }

        public enum WallModes
        {
            INVALID = 0,
            Grid = 1, 
            Freeform = 2,
            Stream = 3
        }
    }

}