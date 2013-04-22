using HackerCentral.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HackerCentral.ViewModels
{
    public class UsersViewModel
    {
        public List<User> Users { get; set; }

        public UsersViewModel()
        {
        }
    }
}