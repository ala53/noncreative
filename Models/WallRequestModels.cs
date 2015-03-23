using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NonCreative.Models
{
    public class GenericWallRequest
    {
        public string Password { get; set; }
        public string KeyPublic { get; set; }
        public string KeyPrivate { get; set; }
    }
    public class WallModelInfoResponse
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Url { get; set; }
        public string OwnerPublic { get; set; }
        public string OwnerName { get; set; }
        public bool HasPassword { get; set; }
        public bool IsPrivate { get; set; }
        public WallModel.WallModes WallMode { get; set; }
        public WallModel.WallAccessPermissionLevels UnauthorizedUsersPermission { get; set; }
        public IEnumerable<WallModelInfoAuthorizedUsersResponse> AuthorizedUsers { get; set; }
        public string BackgroundUrl { get; set; }
        public bool TileBackground { get; set; }
    }
    public class WallModelInfoAuthorizedUsersResponse
    {
        public string Username { get; set; }
        public string KeyPublic { get; set; }
        public WallModel.WallAccessPermissionLevels Permissions { get; set; }
    }
    public class WallModelCreateRequest : GenericWallRequest
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Subtitle { get; set; }
        [Required]
        public string RequestedUrl { get; set; }
        public string WantedPassword { get; set; }
        public string BackgroundUrl { get; set; }
        public bool? TileBackground { get; set; }
        public WallModel.WallModes WallMode { get; set; }
        public WallModel.WallAccessPermissionLevels UnauthorizedUserPermissions { get; set; }

    }
    public class WallChangeBackgroundRequest : GenericWallRequest
    {
        public string NewBackgroundUrl { get; set; }
    }
}