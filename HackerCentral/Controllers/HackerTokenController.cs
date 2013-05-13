using HackerCentral.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace HackerCentral.Controllers
{
    [Authorize]
    public class HackerTokenController : Controller
    {
        //
        // GET: /HackerToken/
        [Authorize(Roles = "User")]
        public ActionResult Register()
        {
            return View();
        }

        //
        // GET: /HackerToken/
        [HttpPost]
        [Authorize(Roles = "User")]
        public ActionResult Register(string token)
        {
            using (var context = new HackerCentralContext())
            {
                HackerToken hackerToken = context.HackerTokens.SingleOrDefault(t => t.Value == token);
                if (hackerToken != null)
                {
                    hackerToken.Consumer = context.UserProfiles.Find(WebSecurity.CurrentUserId);
                    System.Web.Security.Roles.AddUserToRole(User.Identity.Name, UserRole.Hacker.ToString());
                    context.SaveChanges();
                }
            }
            return View();
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult GenerateHackerToken()
        {
            string token = Guid.NewGuid().ToString();
            using (var context = new HackerCentralContext())
            {
                context.HackerTokens.Add(new HackerToken { Value = token, Consumer = null });
                context.SaveChanges();
            }
            ViewBag.token = token;
            return View();
        }
    }
}
