using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using WebMatrix.WebData;

using HackerCentral.Models;


namespace HackerCentral
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {

        protected void Application_Start()
        {
            //WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", true);
            SimpleMembershipInitializer();

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteTable.Routes.MapHubs();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            // create roles if they do not exist
            foreach (UserRole userRole in Enum.GetValues(typeof(UserRole)))
            {
                if (!Roles.RoleExists(userRole.ToString()))
                {
                    Roles.CreateRole(userRole.ToString());
                }
            }

            // seed administrator
            if (!WebSecurity.UserExists(UserRole.Administrator.ToString().ToLowerInvariant()))
            {
                using (var context = new HackerCentralContext(null))
                {
                    context.UserProfiles.Add(new UserProfile
                    {
                        AuthProvider = AuthProvider.Local,
                        FullName = "Peter Griffin",
                        UserName = UserRole.Administrator.ToString().ToLowerInvariant(),
                        UserDiscussion = new HashSet<UserProfileDiscussions>()
                    });
                    context.SaveChanges();
                }
                WebSecurity.CreateAccount(UserRole.Administrator.ToString().ToLowerInvariant(), "secret");
                //WebSecurity.CreateUserAndAccount(UserRole.Administrator.ToString(), "secret");
                //WebSecurity.CreateUserAndAccount(UserRole.Administrator.ToString(), "secret", new { AuthProvider = AuthProvider.Local });
            }
            
            if (!Roles.GetRolesForUser(UserRole.Administrator.ToString().ToLowerInvariant()).Contains(UserRole.Administrator.ToString()))
            {
                Roles.AddUsersToRoles(new[] { UserRole.Administrator.ToString().ToLowerInvariant() }, new[] { UserRole.Administrator.ToString() });
            }
            //add default discussion
            //mark
            using (var context = new HackerCentralContext(null)) 
            { 
               if(!context.Discussions.Any(u => u.ConversationId == AthenaBridgeAPISettings.CONVERSATION_ID)){
                   Discussion newDiscussion = new Discussion
                       {
                           ConversationId = AthenaBridgeAPISettings.CONVERSATION_ID,
                           ApiKey = AthenaBridgeAPISettings.API_KEY,
                           UserId = AthenaBridgeAPISettings.USER_ID,
                           UserDiscussion = new HashSet<UserProfileDiscussions>()
                       };
                   UserProfile admin = context.UserProfiles.SingleOrDefault(u => u.UserName == "administrator");
                   UserProfileDiscussions newRelation = new UserProfileDiscussions { User = admin, RegisteredDiscussion = newDiscussion, BelongTo = Team.Observer };
                   admin.UserDiscussion = admin.UserDiscussion == null ? new HashSet<UserProfileDiscussions>() : admin.UserDiscussion;
                   admin.UserDiscussion.Add(newRelation);
                   newDiscussion.UserDiscussion.Add(newRelation);
                   context.Discussions.Add(newDiscussion);
                   context.UserProfileDiscussions.Add(newRelation);
                   context.SaveChanges();
               }
            }
        }


        private void SimpleMembershipInitializer()
        {
            System.Diagnostics.Debug.WriteLine("SimpleMembershipInitializer()");
            Database.SetInitializer<SimpleContext>(null);
            
            try
            {
                using (var context = new SimpleContext())
                {
                    if (!context.Database.Exists())
                    {
                        // Create the SimpleMembership database without Entity Framework migration schema
                        ((IObjectContextAdapter)context).ObjectContext.CreateDatabase();
                    }
                }

                // TODO: Make sure that it's okay to not call it here if we've already initialized simple membership
                if (!WebSecurity.Initialized)
                    WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("The ASP.NET Simple Membership database could not be initialized. For more information, please see http://go.microsoft.com/fwlink/?LinkId=256588", ex);
            }
        }
    }
}