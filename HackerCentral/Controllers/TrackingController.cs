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

                // Walk through the view models to each EntityTrackVM and call GetCorresponding Entity
                // for each of them. We would do this while the view models are constructed, but it requires
                // accessing the database, so we do it in the controller to maintain the MVC pattern more strictly.
                foreach (var actionTrackVM in model.ActionTracks)
                {
                    foreach (var saveTrackVM in actionTrackVM.SaveTracks)
                    {
                        var userEntity = saveTrackVM.UserEntity;
                        GetCorrespondingEntity(ref userEntity);

                        foreach (var entityTrackVM in saveTrackVM.EntityTracks)
                        {
                            //it's a compile-time error to directly pass variables used in foreach loops by reference
                            var refEntityTrackVM = entityTrackVM;
                            GetCorrespondingEntity(ref refEntityTrackVM);
                        }

                        foreach (var fieldTrackVM in saveTrackVM.FieldTracks)
                        {
                            foreach (var entityTrackVM in saveTrackVM.EntityTracks)
                            {
                                //it's a compile-time error to directly pass variables used in foreach loops by reference
                                var refEntityTrackVM = entityTrackVM;
                                GetCorrespondingEntity(ref refEntityTrackVM);
                            }
                        }
                    }
                }

                return View(model);
            }
        }

        /// <summary>
        /// Uses the tracking data stored in an EntityTrack to look up the corresponding entity
        /// in the database, then stores it in entityTrack.EntityValues.
        /// 
        /// This method has no effect unless entityTrack.EntityValues == null when entityTrack
        /// is passed to this method.
        /// </summary>
        /// <param name="entityTrack">The EntityTrackViewModel to look up in the databse</param>
        private void GetCorrespondingEntity(ref EntityTrackViewModel entityTrack)
        {
            if (entityTrack.EntityValues == null)
            {
                if (entityTrack.EntityType == "UserProfile")
                {
                    var converter = new EntityConverter();
                    var dbEntityTrack = entityTrack.ToDbEntityTrack();
                    entityTrack.EntityValues = converter.GetEntityValues(dbEntityTrack);
                }
                else
                {
                    entityTrack.EntityValues = new Dictionary<string, string>();
                }
            }
        }
    }
}
