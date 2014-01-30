using HackerCentral.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HackerCentral.ViewModels
{
    public class UserEditViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public AuthProvider AuthProvider { get; set; }
        public UserRole Role { get; set; }
        public bool IsBlocked { get; set; }
    }

    public class UserPasswordViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }

        public bool ArePasswordsValid()
        {
            return Password == PasswordConfirmation
                && !String.IsNullOrEmpty(Password)
                && !String.IsNullOrEmpty(PasswordConfirmation);
        }
    }
}