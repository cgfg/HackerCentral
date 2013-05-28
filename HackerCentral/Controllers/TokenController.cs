using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using HackerCentral.Models;
using HackerCentral.Filters;

namespace HackerCentral.Controllers
{
    [HackerCentral.Filters.Authorize]
    public class TokenController : TrackedController
    {
        //
        // GET: /Register/
        [HackerCentral.Filters.Authorize(TypedRoles = new UserRole[] { UserRole.User })]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Register/
        [HttpPost]
        [HackerCentral.Filters.Authorize(TypedRoles = new UserRole[] { UserRole.User })]
        public ActionResult Register(string token)
        {
            using (var context = new HackerCentralContext(this))
            {
                UserProfile userProfile = context.UserProfiles.Find(WebSecurity.CurrentUserId);
                ViewBag.Name = userProfile.FullName;
                HackerToken hackerToken = context.HackerTokens.Include(e => e.Consumers).SingleOrDefault(t => t.Value == token);
                if (hackerToken != null)
                {
                    // For now only allow one token and user to be registered together
                    if (hackerToken.Consumers.Any())
                    {
                        return View("DepletedTokenError");
                    } else {
                        hackerToken.Consumers.Add(userProfile);
                        context.SaveChanges();

                        // must be a the user must have the user role
                        System.Web.Security.Roles.RemoveUserFromRole(User.Identity.Name, UserRole.User.ToString());
                        if (!System.Web.Security.Roles.IsUserInRole(UserRole.Hacker.ToString())) System.Web.Security.Roles.AddUserToRole(User.Identity.Name, UserRole.Hacker.ToString());
                        return View("RegistrationSuccess");
                    }
                }
                else
                {
                    // register a failed token
                    return View("InvalidTokenError");
                }
            }
        }

        //
        // GET: /GenerateToken/
        [HackerCentral.Filters.Authorize(TypedRoles = new UserRole[] { UserRole.Administrator })]
        public ActionResult GenerateToken()
        {
            string token = Guid.NewGuid().ToString();
            using (var context = new HackerCentralContext(this))
            {
                context.HackerTokens.Add(new HackerToken { Value = token });
                context.SaveChanges();
            }
            ViewBag.token = token;
            return View();
        }
    }
}
