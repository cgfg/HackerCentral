using HackerCentral.Extensions;
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
    [HackerCentral.Filters.Authorize(TypedRoles = new UserRole[] { UserRole.Administrator })]
    public class AdministrationController : TrackedController
    {        

        //
        // GET: /ManageUsers/
        public ActionResult ManageUsers(string style = null, string message = null)
        {
            using (var context = new HackerCentralContext(this))
            {
                ViewBag.Style = style;
                ViewBag.Message = message;
                return View(createUserEditList(context));
            }
        }

        [HttpPost]
        public ActionResult EditUser(UserEditViewModel model)
        {
            using (var context = new HackerCentralContext(this))
            {
                UserProfile user = context.UserProfiles.Find(model.UserId);
                if (user == null)
                {
                    ViewBag.Style = "alert-error";
                    ViewBag.Message = string.Format("Modifications failed. The username: {0} does not exist.", model.UserName);
                    return View("ManageUsers", createUserEditList(context));
                }
                else
                {
                    if (user.FullName != model.UserName)
                    {
                        user.FullName = model.FullName;
                        context.SaveChanges();
                    }

                    UserRole role = UserRoleHelper.GetPrimaryUserRole(user.UserName);
                    if (role != model.Role)
                    {
                        Roles.RemoveUserFromRole(user.UserName, role.ToString());
                        Roles.AddUserToRole(user.UserName, model.Role.ToString());
                    }

                    //System.Diagnostics.Debug.WriteLine("db: {0}, model: {1}", UserRoleHelper.IsUserBlocked(user.UserName), model.IsBlocked);

                    if (UserRoleHelper.IsUserBlocked(user.UserName) != model.IsBlocked)
                    {
                        if (model.IsBlocked)
                        {
                            Roles.AddUserToRole(user.UserName, UserRole.Blocked.ToString());
                        }
                        else
                        {
                            Roles.RemoveUserFromRole(user.UserName, UserRole.Blocked.ToString());
                        }
                    }
                    ViewBag.Style = "alert-success";
                    ViewBag.Message = string.Format("Modifications to {0} succeeded.", user.UserName);
                    return View("ManageUsers", createUserEditList(context));
                }
            }
        }

        public PartialViewResult UserEditForm(int id)
        {
            using (var context = new HackerCentralContext(this))
            {
                UserProfile user = context.UserProfiles.Find(id);
                if (user == null)
                {
                    ViewBag.Message = string.Format("The user with id: {0} does not exist.", id);
                    return PartialView("_Error");
                }
                else
                {
                    UserEditViewModel model = new UserEditViewModel
                    {
                        AuthProvider = user.AuthProvider,
                        FullName = user.FullName,
                        IsBlocked = UserRoleHelper.IsUserBlocked(user.UserName),
                        Role = UserRoleHelper.GetPrimaryUserRole(user.UserName),
                        UserId = user.UserId,
                        UserName = user.UserName
                    };
                    return PartialView("_EditUserForm", model);
                }
            }
        }

        // Helper methods
        private IEnumerable<UserEditViewModel> createUserEditList(HackerCentralContext context)
        {
            return context.UserProfiles.AsEnumerable().Select(
                u => new UserEditViewModel
                {
                    AuthProvider = u.AuthProvider,
                    FullName = u.FullName,
                    IsBlocked = UserRoleHelper.IsUserBlocked(u.UserName),
                    Role = UserRoleHelper.GetPrimaryUserRole(u.UserName),
                    UserId = u.UserId,
                    UserName = u.UserName
                }).ToList();
        }
    }
}
