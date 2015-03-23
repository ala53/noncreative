using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace NonCreative.Models
{
    public class AuthKeyModel
    {
        public string KeyPublic { get; set; }
        public string KeyPrivate { get; set; }
    }
}