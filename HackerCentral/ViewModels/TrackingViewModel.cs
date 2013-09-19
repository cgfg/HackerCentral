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
        public string Results { get; set; }
        public DateTime TimeStamp { get; set; }
        public List<SaveTrackViewModel> SaveTracks { get; set; }

        public ActionTrackViewModel() {}
        public ActionTrackViewModel(ActionTrack source)
        {
            ActionName = source.ActionName;
            ControllerName = source.ControllerName;
            Id = source.Id;
            Parameters = source.Parameters;
            Results = source.Results;
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

        public ActionTrack ToDbActionTrack()
        {
            var actionTrack = new ActionTrack();
            actionTrack.ActionName = ActionName;
            actionTrack.ControllerName = ControllerName;
            actionTrack.Id = Id;
            actionTrack.Parameters = Parameters;
            actionTrack.Results = Results;
            actionTrack.TimeStamp = TimeStamp;

            actionTrack.SaveTracks = new List<SaveTrack>(SaveTracks.Count);
            foreach (var saveTrack in SaveTracks)
            {
                actionTrack.SaveTracks.Add(saveTrack.ToDbSaveTrack());
            }

            return actionTrack;
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

        public SaveTrack ToDbSaveTrack()
        {
            var saveTrack = new SaveTrack();

            saveTrack.Id = Id;
            saveTrack.UserEntityTrack = UserEntity.ToDbEntityTrack();

            saveTrack.EntityTracks = new List<EntityTrack>(EntityTracks.Count);
            foreach (var entityTrack in EntityTracks)
            {
                saveTrack.EntityTracks.Add(entityTrack.ToDbEntityTrack());
            }

            saveTrack.FieldTracks = new List<FieldTrack>(FieldTracks.Count);
            foreach (var fieldTrack in FieldTracks)
            {
                saveTrack.FieldTracks.Add(fieldTrack.ToDbFieldTrack());
            }

            return saveTrack;
        }
    }

    public class EntityTrackViewModel
    {
        public int Id { get; set; }
        public string EntityType { get; set; }
        public string EntityId { get; set; }
        public DateTime? TimeRemoved { get; set; }
        public Dictionary<string, string> EntityValues { get; set; }
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

        public EntityTrack ToDbEntityTrack()
        {
            var entityTrack = new EntityTrack();

            entityTrack.Id = Id;
            entityTrack.EntityType = EntityType;
            entityTrack.EntityId = EntityId;
            entityTrack.TimeRemoved = TimeRemoved;

            return entityTrack;
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

        public FieldTrack ToDbFieldTrack()
        {
            var fieldTrack = new FieldTrack();

            fieldTrack.Id = Id;
            fieldTrack.Field = Field;
            fieldTrack.Value = Value;
            fieldTrack.Entity = Entity.ToDbEntityTrack();

            return fieldTrack;
        }
    }
}