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
                        if (userEntity != null)
                        {
                            if (userEntity.EntityType == null)
                            {
                                //This can happen when a new user registers for the first time. The SaveTrack needs
                                //to record that a user intiated a save, but the user doesn't exist yet, so all
                                //of its EntityTrack's values are null.
                                userEntity.EntityValues = new Dictionary<string, string>(1);
                                userEntity.EntityValues.Add("User ID", "[unregistered user]");
                            }
                            else
                            {
                                GetCorrespondingEntity(ref userEntity);
                            }
                        }

                        foreach (var entityTrackVM in saveTrackVM.EntityTracks)
                        {
                            if (entityTrackVM != null)
                            {
                                //it's a compile-time error to directly pass variables used in foreach loops by reference
                                var refEntityTrackVM = entityTrackVM;
                                GetCorrespondingEntity(ref refEntityTrackVM);
                            }
                        }

                        foreach (var fieldTrackVM in saveTrackVM.FieldTracks)
                        {
                            if ((fieldTrackVM != null) && (fieldTrackVM.Entity!= null))
                            {
                                var entity = fieldTrackVM.Entity;
                                GetCorrespondingEntity(ref entity);
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
            if (entityTrack == null)
            {
                throw new NullReferenceException("Can't pass null to GetCorrespondingEntity()");
            }

            if (entityTrack.EntityValues == null)
            {
                var converter = new EntityConverter();
                var dbEntityTrack = entityTrack.ToDbEntityTrack();
                entityTrack.EntityValues = converter.GetEntityValues(dbEntityTrack);
            }
        }
    }
}
