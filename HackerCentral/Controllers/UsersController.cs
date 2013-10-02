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
            List<User> users = ua.GetAllUsers().OrderByDescending(u => u.local_credibility).ToList();
            // Assume data comes in this format: 64,"Lucas Cioffi",0,3,0,1.0,1.0,0
            long id = 0;
            String username = "Average";
            var highly_rated_points_count = users.Sum(u => u.highly_rated_points_count) / users.Count;
            var count_of_average_points_by_participant = users.Sum(u => u.count_of_average_points_by_participant) / users.Count;
            var lowly_rated_points_count = users.Sum(u => u.lowly_rated_points_count) / users.Count;
            var local_credibility = users.Sum(u => u.local_credibility) / users.Count;
            var global_credibility = users.Sum(u => u.global_credibility) / users.Count;
            var all_ratings_by_participant_count = users.Sum(u => u.all_ratings_by_participant_count) / users.Count;
            User averageUser = new User(id+",\""+ username+"\","+highly_rated_points_count+","+count_of_average_points_by_participant+","
                                        +lowly_rated_points_count+","+local_credibility+","+global_credibility+","+all_ratings_by_participant_count);
            users.Add(averageUser);
             var m = new UsersViewModel()
             {
               RankedUsers = users,
                    
             };
            
            return View("Index", m); 
            }
            
        }


    }
}
