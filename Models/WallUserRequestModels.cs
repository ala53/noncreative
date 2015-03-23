using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NonCreative.Models
{
    public class AddOrUpdateAuthorizedUserRequestModel : GenericWallRequest
    {
        public string Username { get; set; }
        public WallModel.WallAccessPermissionLevels PermissionLevel { get; set; }
    }
    public class RemoveAuthorizedUserRequestModel : GenericWallRequest
    {
        public string Username { get; set; }
    }
}