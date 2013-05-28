using HackerCentral.Controllers;
using HackerCentral.Extensions;
using HackerCentral.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HackerCentral.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        public UserRole[] TypedRoles
        {
            get
            {
                return UserRoleHelper.ParseEnum<UserRole>(Roles.Split(',')).ToArray();
            }
            set
            {
                Roles = string.Join(", ", value.Distinct().Select(r => r.ToString()));
            }
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated && !string.IsNullOrWhiteSpace(Roles) && filterContext.HttpContext.User.IsInRole(UserRole.Blocked.ToString()))
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Error",
                    action = "Blocked",
                    resource = string.Format("{0}/{1}", filterContext.ActionDescriptor.ControllerDescriptor.ControllerName, filterContext.ActionDescriptor.ActionName),
                    roles = Roles
                }));
            }
            else
            {
                base.OnAuthorization(filterContext);
            }

        }

        protected override void HandleUnauthorizedRequest(System.Web.Mvc.AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {   
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new {
                    controller = "Error",
                    action = "AccessDenied",
                    resource = string.Format("{0}/{1}", filterContext.ActionDescriptor.ControllerDescriptor.ControllerName, filterContext.ActionDescriptor.ActionName),
                    roles = Roles
                }));
            }
            else
            {
                // user must be signed in
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}