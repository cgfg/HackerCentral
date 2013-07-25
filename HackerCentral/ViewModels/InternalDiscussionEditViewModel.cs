using HackerCentral.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HackerCentral.ViewModels
{
    public class DiscussionEditViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public int DiscussionId { get; set; }
        public Team Role { get; set; }
    }
}
