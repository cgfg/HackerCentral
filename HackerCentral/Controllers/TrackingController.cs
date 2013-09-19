using HackerCentral.Infrastructure.Tracking;
using HackerCentral.Models;
using HackerCentral.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HackerCentral.Controllers
{
    /// <summary>
    /// Show information stored by the tracking system
    /// </summary>
    [HackerCentral.Filters.Authorize(TypedRoles = new UserRole[] { UserRole.Administrator })]
    public class TrackingController : TrackedController
    {
        [HttpGet]
        public ActionResult Data(bool isLimited = true, int numToShow = 10)
        {
            using(SimpleContext dbContext = new HackerCentralContext(this))
            {
                List<ActionTrack> actionTracks;

                if(isLimited)
                {
                    actionTracks = dbContext.ActionTracks
                        .OrderByDescending(a => a.TimeStamp)
                        .Include(a => a.SaveTracks.Select(s => s.EntityTracks))
                        .Include(a => a.SaveTracks.Select(s => s.UserEntityTrack))
                        .Include(a => a.SaveTracks.Select(s => s.FieldTracks.Select(f => f.Entity)))
                        .Take(numToShow).ToList();
                }
                else
                {
                    actionTracks = dbContext.ActionTracks
                        .OrderByDescending(a => a.TimeStamp)
                        .Include(a => a.SaveTracks.Select(s => s.EntityTracks))
                        .Include(a => a.SaveTracks.Select(s => s.UserEntityTrack))
                        .Include(a => a.SaveTracks.Select(s => s.FieldTracks.Select(f => f.Entity)))
                        .ToList();
                }

                var model = new TrackingViewModel()
                {
                    IsLimited = isLimited,
                    ActionTracks = new List<ActionTrackViewModel>(actionTracks.Count)
                };

                foreach (var actionTrack in actionTracks)
                {
                    model.ActionTracks.Add(new ActionTrackViewModel(actionTrack));
                }

                return View(model);
            }
        }

        /// <summary>
        /// Looks up 
        /// </summary>
        /// <param name="entityTrack"></param>
        private void GetCorrespondingEntity(ref EntityTrackViewModel entityTrack)
        {
            var converter = new EntityConverter();
            var dbEntityTrack = entityTrack.ToDbEntityTrack();
            entityTrack.EntityValues = converter.GetEntityValues(dbEntityTrack);
        }
    }
}
