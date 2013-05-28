using HackerCentral.Accessors;
using HackerCentral.Models;
using HackerCentral.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HackerCentral.Controllers
{
    [HackerCentral.Filters.Authorize(TypedRoles = new UserRole[] { UserRole.Hacker, UserRole.Administrator })]
    public class UsersController : TrackedController
    {
        //
        // GET: /Users/
        public ActionResult Index()
        {
            var ua = new UserAccessor();
            var m = new UsersViewModel()
            {
                Users = ua.GetAllUsers()
            };

            return View("Index", m);
        }

    }
}
