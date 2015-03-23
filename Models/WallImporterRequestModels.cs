using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NonCreative.Models
{
    public class PadletImportRequest : GenericWallRequest
    {
        public string CSVData { get; set; }
    }
}