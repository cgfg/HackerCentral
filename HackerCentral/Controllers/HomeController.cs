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
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
                return Dashboard();
            else
                return View("Index");
        }

        public ActionResult Dashboard()
        {
            return View("Dashboard");
        }

        [AllowAnonymous]
        public ActionResult Test()
        {
            var ca = new CommentAccessor();
            

            var b5 = ca.GetCommentsByPoint(376);

            var b4 = ca.GetAllComments();
            long id = (long)b4.First<Comment>().id;

            var x = ca.GetComment(id);

            var y = ca.DestroyComment(id);

            var b6 = ca.GetAllComments();





            return View("Test");
        }
    }
}
