using HackerCentral.Accessors;
using HackerCentral.Models;
using HackerCentral.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace HackerCentral.Controllers
{
    [HackerCentral.Filters.Authorize(TypedRoles = new UserRole[] { UserRole.Hacker, UserRole.Administrator, UserRole.User })]
    public class UsersController : TrackedController
    {
        //
        // GET: /Users/
        //[HackerCentral.Filters.Authorize(TypedRoles = new UserRole[] { UserRole.Hacker, UserRole.Administrator})]
        //public ActionResult Index()
        //{
        //    var ua = new UserAccessor();
        //    var m = new UsersViewModel()
        //    {
        //        Users = ua.GetAllUsers()
        //    };
        //    return View("Index", m);
        //}

        [HackerCentral.Filters.Authorize(TypedRoles = new UserRole[] { UserRole.Hacker, UserRole.Administrator, UserRole.User })]
        public ActionResult Index()
        {
            using(var context = new SimpleContext()){
             var ua = new UserAccessor();
            //var users = ua.GetAllUsers();
            //var userRank = users.OrderByDescending(u => u.local_credibility);
            var m = new UsersViewModel() 
            { 
                RankedUsers = ua.GetAllUsers().OrderByDescending(u => u.local_credibility).ToList(),              
            };
            
            return View("Index", m); 
            }
            
        }


    }
}
