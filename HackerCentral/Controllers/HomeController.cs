using HackerCentral.Accessors;
using HackerCentral.Models;
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
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
                return Dashboard();
            else
                return View("Index");
        }

        public ActionResult Dashboard()
        {
            var pa = new PointAccessor();

            var p = new Point()
            {
                parent_id = 0,
                category = 1,
                summary = "This is my summary",
                full_text = "This is added completely through my C# controller"
            };
            pa.CreatePoint(p);
            
            return View("Dashboard");
        }
    }
}
