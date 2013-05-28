using HackerCentral.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HackerCentral.Controllers
{
    public class ErrorController : TrackedController
    {
        //
        // GET: /AccessDenied/
        [AllowAnonymous]
        public ActionResult AccessDenied(string resource, string roles)
        {
            var model = new AccessDenied { Resource = resource, Roles = roles };
            return View(model);
        }

        //
        // GET: /Blocked/
        [AllowAnonymous]
        public ActionResult Blocked(string resource, string roles)
        {
            var model = new AccessDenied { Resource = resource, Roles = roles };
            return View(model);
        }
    }
}
