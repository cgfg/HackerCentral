using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace HackerCentral.Models
{
    public enum UserRole
    {
        [Description("User")]
        User,
        [Description("Hacker")]
        Hacker,
        [Description("Administrator")]
        Administrator
    };

}