using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NonCreative.Models
{
    public class WallUserReferenceModel
    {
        [System.ComponentModel.DataAnnotations.Key]
        public long Id { get; set; }
        public WallModel.WallAccessPermissionLevels PermissionLevel { get; set; }
        public ApplicationUser User { get; set; }
    }
}