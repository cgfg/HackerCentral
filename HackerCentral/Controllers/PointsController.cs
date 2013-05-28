using HackerCentral.Accessors;
using HackerCentral.Models;
using HackerCentral.ViewModels;
using HackerCentral.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HackerCentral.Controllers
{
    [HackerCentral.Filters.Authorize(TypedRoles = new UserRole[] { UserRole.Hacker, UserRole.Administrator })]
    public class PointsController : TrackedController
    {
        public ActionResult Index(string message = "")
        {
            var pa = new PointAccessor();
            var model = new PointsViewModel(pa.GetAllPoints());

            ViewBag.Message = message;
            return View("Index", model);
        }

        [HttpPost]
        public ActionResult Create(Point p)
        {
            var pa = new PointAccessor();
            if (pa.CreatePoint(p))
                return Index("Point successfully created");
            else
                return Index("Point not successfully created");
        }

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
            if (pa.UpdatePoint(p))
                return Index("Point successfully updated");
            else
                return Index("Point was not successfully updated");
        }

        public PartialViewResult EditForm(long id)
        {
            var pa = new PointAccessor();
            return PartialView("_EditPoint", pa.GetPoint(id));
        }
    }
}
