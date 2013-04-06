using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace HackerCentral.Models
{
    public enum UserRole { 
        [Description("HACKER")]
        HACKER,
        [Description("ADMINISTRATOR")]
        ADMINISTRATOR };

}