using HackerCentral.Accessors;
using HackerCentral.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HackerCentral.Controllers
{
    public class UsersController : Controller
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
