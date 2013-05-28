using HackerCentral.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;


namespace HackerCentral.Controllers
{
    public abstract class TrackedController : Controller
    {
        public int? ActionId
        {
            get;
            private set;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Dictionary<string, object> parameterDictionary = new Dictionary<string, object>();
            foreach (var parameter in filterContext.ActionDescriptor.GetParameters())
            {
                var valueProvider = filterContext.Controller.ValueProvider.GetValue(parameter.ParameterName);
                parameterDictionary.Add(parameter.ParameterName, valueProvider == null ? null : valueProvider.AttemptedValue);
            }

            using (var context = new SimpleContext())
            {
                ActionTrack actionTrack = context.ActionTracks.Add(new ActionTrack
                {
                    TimeStamp = DateTime.UtcNow,
                    ControllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                    ActionName = filterContext.ActionDescriptor.ActionName,
                    Parameters = JsonConvert.SerializeObject(parameterDictionary)
                });
                context.SaveChanges();
                ActionId = actionTrack.Id;
            }

            base.OnActionExecuting(filterContext);
        }

        /* ContentResult
         * EmptyResult
         * FileResult
         * UnauthorizedResult
         * JavaScriptResult
         * JsonResult
         * RedirectResult
         * RedirectToRouteResult
         * ViewResultBase
         */
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // ActionId should not be null
            if (ActionId.HasValue)
            {
                using (var context = new SimpleContext())
                {
                    ActionTrack actionTrack = context.ActionTracks.Find(ActionId);
                    actionTrack.Results = JsonConvert.SerializeObject(filterContext.Result);
                    context.SaveChanges();
                }
            }
            base.OnActionExecuted(filterContext);
        }
    }
}
