using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace HackerCentral.Models
{
    public enum SecondaryUserRole
    {
        Blocked = 0
    }

    public enum PrimaryUserRole
    {
        User = 1,
        Hacker = 2,
        Administrator = 3
    }

    public enum UserRole
    {
        [Description("Blocked")]
        Blocked = SecondaryUserRole.Blocked,
        [Description("User")]
        User = PrimaryUserRole.User,
        [Description("Hacker")]
        Hacker = PrimaryUserRole.Hacker,
        [Description("Administrator")]
        Administrator = PrimaryUserRole.Administrator,
    }
}