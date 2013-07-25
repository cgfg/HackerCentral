using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using Newtonsoft.Json;
using System.Data;
using System.Data.Objects;
using System.Data.Entity.Infrastructure;
using WebMatrix.WebData;
using System.Data.Objects.DataClasses;
using HackerCentral.Controllers;

namespace HackerCentral.Models
{
    // reflect on this in SaveChanges() to through away external modifications to an entity or entity property
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
    public class ReadonlyAttribute : Attribute { }

    // reflect on this in SaveChanges() to through away external modifications to an entity property of type string
    // and in order to auto set the property with guid constant over the entities lifecycle
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AutoGuidLabelAttribute : ReadonlyAttribute { }

    public class SimpleContext : DbContext
    {
        public SimpleContext() : base("DefaultConnection") { }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<HackerToken> HackerTokens { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Discussion> Discussions { get; set; }
        public DbSet<UserProfileDiscussions> UserProfileDiscussions { get; set; }
        public DbSet<ActionTrack> ActionTracks { get; set; }
        public DbSet<SaveTrack> SaveTracks { get; set; }
        public DbSet<EntityTrack> EntityTracks { get; set; }
        public DbSet<FieldTrack> FieldTracks { get; set; }
    }
       

    public class HackerCentralContext : SimpleContext
    {
        private static readonly EntityState ChangedState = EntityState.Added | EntityState.Deleted | EntityState.Modified;

        private TrackedController controller;
        //private readonly ObjectContext objectContext;

        public HackerCentralContext(TrackedController controller) 
        {
            // If using the dbcontext has too much overhead or costs to much dev time
            // default to using ObjectContext and its events
            //objectContext = ((IObjectContextAdapter)this).ObjectContext;

            // these exceptions can be removed but then SaveChanges() should be modified to reflect
            // the additional options
            // if (controller == null) throw new ArgumentNullException("controller");
            // if (controller.ActionId == null) throw new ArgumentNullException("controller.ActionId");

            this.controller = controller;
        }

        #region Helper methods
        private string FindEntityTrackId(DbEntityEntry entry)
        {
            return string.Join(",", entry.Entity.GetType().GetProperties()
                .Where(p => p.GetCustomAttributes(true).OfType<KeyAttribute>().Any()) // is a key
                .OrderBy(p => p.Name)
                .Select(p => entry.OriginalValues[p.Name].ToString()).ToArray());
        }

        private EntityTrack FindEntityTrack(SimpleContext context, DbEntityEntry entry)
        {
            string entityId = FindEntityTrackId(entry);

            // if there is an error here then tracking has lost consistancy
            return context.EntityTracks.AsEnumerable()
                .SingleOrDefault(e => e.EntityId == entityId // same id
                    && e.EntityType == entry.Entity.GetType().Name // same type
                    && !e.TimeRemoved.HasValue); // not delted
        }
        #endregion

        // reflect on readonly and autoguidlabel attribute to provide there functionality
        // note to self: I have decided to user a work around for the tracking in order to
        // save time by avoiding writting a generalized solution using the attribute 
        // approach 
        public override int SaveChanges()
        {
            int returnValue;
            //copy list of entries since may make changes
            ChangeTracker.DetectChanges();
            var entries = ChangeTracker.Entries();

            List<FieldTrack> fieldTrackList = new List<FieldTrack>();
            List<EntityTrack> entityTrackList = new List<EntityTrack>();
            Dictionary<EntityTrack, DbEntityEntry> entityMap = new Dictionary<EntityTrack, DbEntityEntry>();

            using (var context = new SimpleContext())
            {
                foreach (var entry in entries)
                {
                    // some of the code for attribute based tracking
                    // through away changes to properties with read only attributes
                    //foreach (string propertyName in entry.Entity.GetType().GetProperties()
                    //        .Where(p => entry.Property(p.Name).IsModified
                    //            && p.GetCustomAttributes(false).OfType<ReadonlyAttribute>().Any())
                    //        .Select(p => p.Name))
                    //{
                    //    entry.Property(propertyName).IsModified = false;
                    //}
                    // through away changes to entities with read only attributes
                    //if (ChangedState.HasFlag(entry.State) && entry.Entity.GetType().GetCustomAttributes(true).OfType<ReadonlyAttribute>().Any())
                    //{
                    //    entry.State = EntityState.Unchanged;
                    //}

                    var names = entry.Entity.GetType().GetProperties().Where(p => p.PropertyType.GUID == typeof(ICollection<>).GUID).Select(p => p.Name);

                    //System.Diagnostics.Debug.WriteLine("[{0}], [{1}]", string.Join(",", names), string.Join(",", names.Where(n => entry.Collection(n).CurrentValue != null).Select(n => entry.Collection(n).CurrentValue.GetType().Name)));

                    EntityTrack entityTrack = null;
                    switch (entry.State)
                    {
                        case System.Data.EntityState.Added:
                            if (entry.Entity.GetType().GetCustomAttributes(true).OfType<ReadonlyAttribute>().Any())
                            {
                                entry.State = EntityState.Unchanged;
                            }
                            entityTrack = new EntityTrack { EntityType = entry.Entity.GetType().Name };
                            entityMap.Add(entityTrack, entry);
                            entityTrackList.Add(entityTrack);
                            //context.EntityTracks.Add(entityTrack);
                            foreach (var field in entry.CurrentValues.PropertyNames)
                            {
                                fieldTrackList.Add(new FieldTrack
                                //context.FieldTracks.Add(new FieldTrack
                                {
                                    Entity = entityTrack,
                                    Field = field,
                                    Value = JsonConvert.SerializeObject(entry.CurrentValues[field])
                                });
                            }
                            break;
                        case System.Data.EntityState.Modified:
                            entityTrack = FindEntityTrack(context, entry);
                            if (entityTrack == null)
                            {
                                entityTrack = new EntityTrack { EntityId = FindEntityTrackId(entry), EntityType = entry.Entity.GetType().Name };
                                foreach (var field in entry.CurrentValues.PropertyNames)
                                {
                                    fieldTrackList.Add(new FieldTrack
                                    //context.FieldTracks.Add(new FieldTrack
                                    {
                                        Entity = entityTrack,
                                        Field = field,
                                        Value = JsonConvert.SerializeObject(entry.CurrentValues[field])
                                    });
                                }
                            }
                            else
                            {
                                foreach (var field in entry.CurrentValues.PropertyNames)
                                {
                                    if (entry.Property(field).IsModified)
                                    {
                                        fieldTrackList.Add(new FieldTrack
                                        //context.FieldTracks.Add(new FieldTrack
                                        {
                                            Entity = entityTrack,
                                            Field = field,
                                            Value = JsonConvert.SerializeObject(entry.CurrentValues[field])
                                        });
                                    }
                                }
                            }
                            entityTrackList.Add(entityTrack);
                            //context.EntityTracks.Add(entityTrack);
                            break;
                        case System.Data.EntityState.Deleted:
                            entityTrack = FindEntityTrack(context, entry);
                            entityTrack.TimeRemoved = DateTime.UtcNow;
                            entityTrackList.Add(entityTrack);
                            //context.EntityTracks.Add(entityTrack);
                            break;
                    }
                }

                returnValue = base.SaveChanges();

                if (entityTrackList.Any())
                {
                    EntityTrack userEntityTrack = null;

                    bool isAuthenticated = false;
                    try
                    {
                        isAuthenticated = WebSecurity.IsAuthenticated;
                    }
                    catch { }

                    if (isAuthenticated)
                    {
                        userEntityTrack = FindEntityTrack(context, context.Entry(context.UserProfiles.Find(WebSecurity.CurrentUserId)));
                    }

                    SaveTrack saveTrack = new SaveTrack
                    {
                        UserEntityTrack = userEntityTrack,
                        EntityTracks = entityTrackList,
                        FieldTracks = fieldTrackList,
                    };

                    foreach (var pair in entityMap)
                    {
                        pair.Key.EntityId = FindEntityTrackId(pair.Value);
                    }
                    
                    // it is excpected that controller and actionid are not null
                    ActionTrack actionTrack = null;
                    if (controller != null && controller.ActionId.HasValue)
                    {
                        actionTrack = context.ActionTracks.Include(a => a.SaveTracks).SingleOrDefault(a => a.Id == controller.ActionId);
                        actionTrack.SaveTracks.Add(saveTrack);
                    }
                    else
                    {
                        context.SaveTracks.Add(saveTrack);
                    }

                    context.SaveChanges();
                }
            }

            return returnValue;
        }
    }

    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        //[AutoGuidLabel]
        //public string Guid { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public AuthProvider AuthProvider { get; set; }
        public ICollection<UserProfileDiscussions> UserDiscussion { get; set; }
        public ICollection<HackerToken> RegisteredTokens { get; set; }

    }

    [Table("Discussion")]
    public class Discussion
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int DiscussionId { get; set; }
        public int ConversationId { get; set; }
        public string ApiKey { get; set; }
        public int UserId { get; set; }
        public ICollection<UserProfileDiscussions> UserDiscussion { get; set; }
    }

    [Table("UserProfileDiscussions")]
    public class UserProfileDiscussions
    {
        [Key, Column(Order = 0)]
        public int UserId { get; set; }
        [Key, Column(Order = 1)]
        public int DiscussionId { get; set; }
        public UserProfile User {get; set;}
        public Discussion RegisteredDiscussion { get; set; } 
        public Team BelongTo { get; set; }
    }


    [Table("HackerToken")]
    public class HackerToken
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Value { get; set; }
        public ICollection<UserProfile> Consumers { get; set; }
    }

    [Table("Message")]
    public class Message
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public UserProfile Sender { get; set; }
        public string Text { get; set; }
        public ICollection<Delivery> Deliveries { get; set; }
    }

    [Table("Delivery")]
    public class Delivery
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime? TimeDelivered { get; set; }
        public UserProfile Reciever { get; set; }
        public Message Message { get; set; }
    }

    [Readonly]
    [Table("ActionTrack")]
    public class ActionTrack
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }

        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public string Parameters { get; set; }
        public string Results { get; set; }

        public ICollection<SaveTrack> SaveTracks { get; set; }
    }

    [Readonly]
    [Table("SaveTrack")]
    public class SaveTrack
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // this is the work around for the not using attributes
        public EntityTrack UserEntityTrack { get; set; }

        public ICollection<EntityTrack> EntityTracks { get; set; }
        public ICollection<FieldTrack> FieldTracks { get; set; }
    }

    [Readonly]
    [Table("EntityTrack")]
    public class EntityTrack
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // EntityId is a string containing the comma delimited 
        // concatination of the primary key components of the
        // entity in alphabetic order of each components name
        public string EntityId { get; set; }
        public string EntityType { get; set; }
        public DateTime? TimeRemoved { get; set; }
    }

    [Readonly]
    [Table("FieldTrack")]
    public class FieldTrack
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Field { get; set; }
        public string Value { get; set; }
        public EntityTrack Entity { get; set; }
    }

    public enum AuthProvider
    {
        Facebook,
        Google,
        LinkedIn,
        Microsoft,
        Twitter,
        Yahoo,
        Local
    }

    public enum Team
    {
        Pro,
        Con,
        Obs // observe
    }
}