using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NonCreative.Models
{
    public class WallChunkRequest : GenericWallRequest
    {
        [Required]
        public int Beginning { get; set; }
        [Required]
        public int Count { get; set; }
    }
    public class WallModelAddPostRequest : GenericWallRequest
    {
        public string Header { get; set; }
        public string Content { get; set; }
        public string AttachmentUrl { get; set; }
    }
    public class WallModelDeletePostRequest : GenericWallRequest
    {
        [Required]
        public long PostId { get; set; }
    }
    public class WallModelEditPostRequest : GenericWallRequest
    {
        [Required]
        public long PostId { get; set; }
        public string NewHeader { get; set; }
        public string NewContent { get; set; }
        public string NewAttachment { get; set; }
        public int NewX { get; set; }
        public int NewY { get; set; }
    }
    public class WallModelPostInfoResponse
    {
        public long PostId { get; set; }
        public long SortOrder { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Author { get; set; }
        public string Header { get; set; }
        public string Content { get; set; }
        public string KeyPublic { get; set; }
        public string Attachment { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }

}