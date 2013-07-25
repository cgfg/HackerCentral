using HackerCentral.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HackerCentral.ViewModels
{
    public class DiscussionViewModel
    {
        public List<UserProfile> Users { get; set; }

        public DiscussionViewModel()
        {
        }
    }
}