using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NonCreative.Models
{
    public class FileUploadModel
    {
        [System.ComponentModel.DataAnnotations.Key]
        public long FileId { get; set; }
        public string Filename { get; set; }
        public virtual WallModel Wall { get; set; }
        public string WallId { get; set; }
    }
}