using HackerCentral.Accessors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HackerCentral.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {

        public ActionResult Index()
        {
            var ua = new UserAccessor();
            var users = ua.GetAllUsers();
            return View();
        }

    }
}
