using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HackerCentral.Models
{
    public class AccessDenied
    {
        public string Resource {get; set;}
        public string Roles { get; set; }
    }
}