using HackerCentral.Accessors;
using HackerCentral.Models;
using HackerCentral.ViewModels;
using HackerCentral.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace HackerCentral.Controllers
{
    [HackerCentral.Filters.Authorize(TypedRoles = new UserRole[] { UserRole.Hacker, UserRole.Administrator, UserRole.User })]
    public class PointsController : TrackedController
    {
        public ActionResult Index(string message = "")
        {
            var pa = new PointAccessor();
            var ua = new UserAccessor();
            var model = new PointsViewModel(pa.GetAllPoints(), User.Identity.Name, ua.getUserId(User.Identity.Name));
            
            ViewBag.Message = message;
            return View("Index", model);
        }

        [HttpGet]
        public string Search(string username)
        {
            var pa = new PointAccessor();
            var ua = new UserAccessor();
            long id = ua.getUserId(username);
            string userPointsId = "";
            foreach (long pointId in pa.GetAllPoints().Where(p => p.user_id == id).Select(p => p.id).ToList()){
                userPointsId = userPointsId + pointId.ToString() + ",";
            }
            return userPointsId;
        }

        [HackerCentral.Filters.Authorize(TypedRoles = new UserRole[] { UserRole.Hacker, UserRole.Administrator})]
        [HttpPost]
        public ActionResult Create(Point p)
        {
            var pa = new PointAccessor();
            if (pa.CreatePoint(p))
                return Index("Point successfully created");
            else
                return Index("Point not successfully created");
        }

        [HackerCentral.Filters.Authorize(TypedRoles = new UserRole[] { UserRole.Hacker, UserRole.Administrator })]
        [HttpGet]
        public ActionResult Destroy(long id)
        {
            var pa = new PointAccessor();
            if (pa.DestroyPoint(id))
                return Index("Point successfully deleted");
            else
                return Index("Point did not delete successfully");
        }

        [HttpPost]
        public ActionResult Edit(Point p)
        {
            var pa = new PointAccessor();
            if (pa.UpdatePoint(p, User.Identity.Name))
                return Index("Point successfully updated");
            else
                return Index("Point was not successfully updated");
        }

        [HackerCentral.Filters.Authorize(TypedRoles = new UserRole[] { UserRole.Hacker, UserRole.Administrator })]
        public PartialViewResult EditForm(long id)
        {
            var pa = new PointAccessor();
            return PartialView("_EditPoint", pa.GetPoint(id));
        }

        public PartialViewResult LinkForm(long id)
        {
            var pa = new PointAccessor();
            return PartialView("_LinkPoint", pa.GetPoint(id));
        }

        public ActionResult ViewPoint(long id)
        {
            ViewBag.IFrame = string.Format("http://129.93.238.144/embedded/77/{0}/true", id);
            return View("ViewPoint");
        }
    }
}
