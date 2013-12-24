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
using System.Configuration;

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

        public ActionResult Discussion()
        {
            ViewBag.IFrame = string.Format("http://129.93.238.144/{0}", ConfigurationManager.AppSettings["ConversationId"]);
            return View("Discussion");
        }
    }
}
