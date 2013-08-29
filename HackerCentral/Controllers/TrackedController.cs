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
    /// <summary>
    /// <c>TrackedController</c> is an abstract class which adds tracking functionality at the 
    /// control layer. Any controller which inherits from <c>TrackedController</c> in place of 
    /// <c>Controller</c> gains the ability to track all requests made to the controller.
    /// </summary>
    /// <remarks>
    /// The current implementation serializes all controller action request parameters and its 
    /// results into json. This is very hard to manipulate and query. It may be of interest to 
    /// modify the functionality of this class to store more targeted data about the controller
    /// action request parameters and its result. Some comments regarding this can be found before
    /// the code implementing <c>OnActionExecuted</c>.
    /// </remarks>
    public abstract class TrackedController : Controller
    {
        /// <summary>
        /// Keeps track of the most recently generated action id. Action ids identify a particular
        /// entry for a <c>ActionTrack</c> entity in the database. The <c>ActionId</c> property
        /// is updated when a new conroller action is started. The <c>ActionId</c> is consumed by
        /// the <c>OnActionExecuted</c> method.
        /// </summary>
        public int? ActionId
        {
            get;
            private set;
        }

        /// <summary>
        /// This method is invoked just before a controller action is started. The 
        /// <c>filterContext</c> parameter contains all contextual information for the action. 
        /// The <c>TrackedController</c> class overrides prepends functionality to the default 
        /// functionality. The prepended functionality extracts the parameters passed to the 
        /// action from the <c>filterContext</c> parameter before encoding the parameters into 
        /// json and storing the json to a tracking entry in the database.
        /// </summary>
        /// <param name="filterContext"></param>
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

        /* The following is a list of all result types:
         * 1. ContentResult
         * 2. EmptyResult
         * 3. FileResult
         * 4. UnauthorizedResult
         * 5. JavaScriptResult
         * 6. JsonResult
         * 7. RedirectResult
         * 8. RedirectToRouteResult
         * 9. ViewResultBase
         * it might be of interest to treat all result types individually to create a more readable
         * database entry for the results of an action.
         */
        /// <summary>
        /// This method is invoked just after a controller action a taken. Unlike the 
        /// <c>OnActionExecuting</c> method the <c>filterContext</c> parameter of this method 
        /// contains the result of the taking the action. The <c>TrackedController</c> class 
        /// prepends functionality to the default functionality of this method. The prepended
        /// functionality store the result of tacking the controller action into the entity
        /// generated in the <c>OnActionExecuting</c> method. The results are serialized into json
        /// making it difficult to read or query its content using sql.
        /// </summary>
        /// <param name="filterContext"></param>
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
