using HackerCentral.Extensions;
using HackerCentral.Models;
using HackerCentral.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Entity;
using WebMatrix.WebData;
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

        public ActionResult ManageDiscussion( string style = null, string message = null)
        {
            using (var context = new HackerCentralContext(this))
            {
                ViewBag.Style = style;
                ViewBag.Message = message;
                int discussionId = context.Discussions.SingleOrDefault(d => d.ConversationId == 77).DiscussionId;
                return View(createDiscussionEditList(context, discussionId));
            }
        }

        //public ActionResult ManageUsers(string style = null, string message = null)
        //{
        //    using (var context = new HackerCentralContext(this))
        //    {
        //        ViewBag.Style = style;
        //        ViewBag.Message = message;
        //        return View(createManageDiscussionList(context));
        //    }
        //}

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUserPassword(UserPasswordViewModel model)
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
                else if(!model.ArePasswordsValid())
                {
                    ViewBag.Style = "alert-error";
                    ViewBag.Message = string.Format("The new passwords didn't match or were black. The password for {0} was not changed.", model.UserName);
                    return View("ManageUsers", createUserEditList(context));
                }
                else
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        var resetPasswordToken = WebSecurity.GeneratePasswordResetToken(model.UserName);
                        changePasswordSucceeded = WebSecurity.ResetPassword(resetPasswordToken, model.Password);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        ViewBag.Style = "alert-success";
                        ViewBag.Message = string.Format("Password changed for {0}.", user.UserName);
                        return View("ManageUsers", createUserEditList(context));
                    }
                    else
                    {
                        ViewBag.Style = "alert-error";
                        ViewBag.Message = string.Format("There was an error changing the password for {0}.", model.UserName);
                        return View("ManageUsers", createUserEditList(context));
                    }
                }
            }
        }

        public PartialViewResult UserEditPasswordForm(int id)
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
                    UserPasswordViewModel model = new UserPasswordViewModel
                    {
                        UserId = user.UserId,
                        UserName = user.UserName,
                        Password = "",
                        PasswordConfirmation = ""
                    };
                    return PartialView("_EditUserPasswordForm", model);
                }
            }
        }

        [HttpPost]
        public ActionResult EditDiscussion(DiscussionEditViewModel model)
        {
            using (var context = new HackerCentralContext(this))
            {
                
                UserProfileDiscussions userDiscussion = context.UserProfileDiscussions.Find(model.UserId, model.DiscussionId);
                if (userDiscussion == null)
                {
                    ViewBag.Style = "alert-error";
                    ViewBag.Message = string.Format("Modifications failed. The Discussion does not exist.");
                    return View("ManageDiscussion", createDiscussionEditList(context, model.DiscussionId));
                }
                else
                {                             
                    if(userDiscussion.BelongTo != model.Role)
                    {
                        userDiscussion.BelongTo = model.Role;
                        context.SaveChanges();
                    }
                    ViewBag.Style = "alert-success";
                    ViewBag.Message = string.Format("{0} is now enrolled as a {1}.", model.FullName, model.Role.ToString());
                    return View("ManageDiscussion", createDiscussionEditList(context, model.DiscussionId));
                }
            }
        }

        public PartialViewResult DiscussionEditForm(int userId, int discussionId)
        {
            using (var context = new HackerCentralContext(this))
            {
                UserProfile user = context.UserProfiles.Include(u => u.UserDiscussion).SingleOrDefault(u => u.UserId == userId);
           
                if (user == null)
                {
                    ViewBag.Message = string.Format("The user with id: {0} does not exist.", userId);
                    return PartialView("_Error");
                }
                else
                {
                    DiscussionEditViewModel model = new DiscussionEditViewModel
                    {
                        FullName = user.FullName,
                        Role = user.UserDiscussion.SingleOrDefault(u => u.discussionId == discussionId).BelongTo,                  
                        UserId = userId,
                        DiscussionId = discussionId
                    };
                    return PartialView("_EditDiscussionForm", model);
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

        private IEnumerable<DiscussionEditViewModel> createDiscussionEditList(HackerCentralContext context, int discussionId)
        {
            return context.UserProfileDiscussions.Where(ud => ud.discussionId == discussionId).Include(ud => ud.User).Select(
                ud => new DiscussionEditViewModel
                {   
                    FullName = ud.User.FullName,          
                    Role = ud.BelongTo,
                    UserId = ud.userProfileId,
                    DiscussionId = discussionId
                }).ToList();
        }
    }
}
