using HackerCentral.Accessors;
using HackerCentral.Models;
using HackerCentral.ViewModels;
using HackerCentral.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix;
using WebMatrix.WebData;

namespace HackerCentral.Controllers
{

    [HackerCentral.Filters.Authorize]
    public class HomeController : TrackedController
    {
        
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
                return Dashboard();
            else
                return View("Index");
        }

        public ActionResult Dashboard()
        {
            DashboardViewModel model = null;
            using (var context = new HackerCentralContext(this))
            {
                model = new DashboardViewModel { UserProfile = context.UserProfiles.Find(WebSecurity.CurrentUserId) };
            }
             
            return View("Dashboard", model);
        }

        [AllowAnonymous]
        public ActionResult Test()
        {
            return View("Test");
        }
    }
}
