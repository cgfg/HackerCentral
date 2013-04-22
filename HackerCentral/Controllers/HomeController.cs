using HackerCentral.Accessors;
using HackerCentral.Models;
using HackerCentral.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace HackerCentral.Controllers
{

    [Authorize]
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index(bool adminMode = false)
        {
            if (Request.IsAuthenticated)
                return Embedded(adminMode);
            else
                return View("Index");
        }

        public ActionResult Embedded(bool adminMode = false)
        {
            var m = new EmbeddedViewModel()
            {
                adminMode = adminMode,
                adminTitle = "Admin Mode",
                normalTitle = "UNL Research"
            };

            return View("Embedded", m);
        }

        [AllowAnonymous]
        public ActionResult Test()
        {
            return View("Test");
        }
    }
}
