using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace NonCreative.Models
{
    public class WallPost
    {
        [System.ComponentModel.DataAnnotations.Key, Required]
        public long PostId { get; set; }
        public long SortOrder { get; set; }
        public int XPosition { get; set; }
        public int YPosition { get; set; }
        [Required]
        public string CreatorName { get; set; }
        public string Header { get; set; }
        public string Content { get; set; }
        public string Attachment { get; set; }
        [Required]
        public DateTime CreationTime { get; set; }
        [Required]
        public DateTime UpdateTime { get; set; }
        [XmlIgnore, Required]
        public string CreatorPrivate { get; set; }
        [Required]
        public string CreatorPublic { get; set; }

        //Foreign Key
        public string WallId { get; set; }
        public WallModel Wall { get; set; }
    }
}