using NonCreative.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NonCreative.Models
{
    public class UserWallReference
    {
        [Key]
        public long Id { get; set; }
        public string WallId { get; set; }
        [ForeignKey("WallId")]
        public WallModel Wall { get; set; }
    }
    public class UserPostReference
    {
        [Key]
        public long Id { get; set; }
        public long PostId { get; set; }
        [ForeignKey("PostId")]
        public WallPost Post { get; set; }
    }
}