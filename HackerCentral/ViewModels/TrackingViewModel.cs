using HackerCentral.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HackerCentral.ViewModels
{
    public class TrackingViewModel
    {
        public List<ActionTrackViewModel> ActionTracks { get; set; }
        public bool IsLimited { get; set; }
        public int NumActionsShown
        {
            get
            {
                return ActionTracks.Count;
            }
        }
    }

    public class ActionTrackViewModel
    {
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public int Id { get; set; }
        public string Parameters { get; set; }
        public DateTime TimeStamp { get; set; }
        public List<SaveTrackViewModel> SaveTracks { get; set; }

        public ActionTrackViewModel() {}
        public ActionTrackViewModel(ActionTrack source)
        {
            ActionName = source.ActionName;
            ControllerName = source.ControllerName;
            Id = source.Id;
            Parameters = source.Parameters;
            TimeStamp = source.TimeStamp;

            if (source.SaveTracks == null)
            {
                SaveTracks = new List<SaveTrackViewModel>();
            }
            else
            {
                SaveTracks = new List<SaveTrackViewModel>(source.SaveTracks.Count);
                foreach (var saveTrack in source.SaveTracks)
                {
                    SaveTracks.Add(new SaveTrackViewModel(saveTrack));
                }
            }
        }
    }

    public class SaveTrackViewModel
    {
        public int Id { get; set; }
        public EntityTrackViewModel UserEntity { get; set; }
        public List<EntityTrackViewModel> EntityTracks { get; set; }
        public List<FieldTrackViewModel> FieldTracks { get; set; }

        public SaveTrackViewModel() { }
        public SaveTrackViewModel(SaveTrack source)
        {
            Id = source.Id;
            if (source.UserEntityTrack == null)
            {
                UserEntity = new EntityTrackViewModel();
            }
            else
            {
                UserEntity = new EntityTrackViewModel(source.UserEntityTrack);
            }

            if (source.EntityTracks == null)
            {
                EntityTracks = new List<EntityTrackViewModel>();
            }
            else
            {
                EntityTracks = new List<EntityTrackViewModel>(source.EntityTracks.Count);
                foreach (var entityTrack in source.EntityTracks)
                {
                    EntityTracks.Add(new EntityTrackViewModel(entityTrack));
                }
            }

            if (source.FieldTracks == null)
            {
                FieldTracks = new List<FieldTrackViewModel>();
            }
            else
            {
                FieldTracks = new List<FieldTrackViewModel>(source.FieldTracks.Count);
                foreach (var fieldTrack in source.FieldTracks)
                {
                    FieldTracks.Add(new FieldTrackViewModel(fieldTrack, EntityTracks));
                }
            }
        }
    }

    public class EntityTrackViewModel
    {
        public int Id { get; set; }
        public string EntityType { get; set; }
        public string EntityId { get; set; }
        public DateTime? TimeRemoved { get; set; }
        public bool WasDeleted
        {
            get
            {
                return TimeRemoved.HasValue;
            }
        }

        public EntityTrackViewModel() { }
        public EntityTrackViewModel(EntityTrack source)
        {
            Id = source.Id;
            EntityType = source.EntityType;
            EntityId = source.EntityId;
            TimeRemoved = source.TimeRemoved;
        }
    }

    public class FieldTrackViewModel
    {
        public int Id { get; set; }
        public string Field { get; set; }
        public string Value { get; set; }
        public EntityTrackViewModel Entity { get; set; }

        public FieldTrackViewModel() { }
        public FieldTrackViewModel(FieldTrack source)
        {
            Id = source.Id;
            Field = source.Field;
            Value = source.Value;
            Entity = new EntityTrackViewModel(source.Entity);
        }

        public FieldTrackViewModel(FieldTrack source, List<EntityTrackViewModel> generatedEntityTracks)
        {
            Id = source.Id;
            Field = source.Field;
            Value = source.Value;

            var matchedEntity = generatedEntityTracks.Find(e => e.Id == source.Entity.Id);
            if (matchedEntity == null)
            {
                Entity = new EntityTrackViewModel(source.Entity);
            }
            else
            {
                Entity = matchedEntity;
            }
        }
    }
}