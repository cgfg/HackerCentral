using HackerCentral.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HackerCentral.Infrastructure.Tracking.ConverterStrategies
{
    public class UserProfileDiscussionsConverterStrategy : IConverterStrategy
    {
        public Object ConvertEntityTrack(EntityTrack entityTrack)
        {
            return FetchUserProfileDiscussions(entityTrack);
        }

        public Dictionary<string, string> GetEntityValues(EntityTrack entityTrack)
        {
            var userProfileDiscussions = FetchUserProfileDiscussions(entityTrack);
            var dictionary = new Dictionary<string, string>(5);

            dictionary.Add("Discussion ID", userProfileDiscussions.discussionId.ToString());
            dictionary.Add("User Profile ID", userProfileDiscussions.userProfileId.ToString());

            if (userProfileDiscussions.RegisteredDiscussion == null)
            {
                dictionary.Add("Registered Discussion", "[null]");
            }
            else
            {
                dictionary.Add("Registered Discussion", userProfileDiscussions.RegisteredDiscussion.DiscussionId.ToString());
            }

            if (userProfileDiscussions.RegisteredDiscussion == null)
            {
                dictionary.Add("User", "[null]");
            }
            else
            {
                dictionary.Add("User", userProfileDiscussions.User.UserId.ToString());
            }

            dictionary.Add("Team", userProfileDiscussions.BelongTo.ToString());

            return dictionary;
        }

        private UserProfileDiscussions FetchUserProfileDiscussions(EntityTrack entityTrack)
        {
            using (SimpleContext dbContext = new HackerCentralContext(null))
            {
                string idStr = entityTrack.EntityId;
                var idParts = idStr.Split(',');
                var ids = idParts.Select<string, int>(id => Int32.Parse(id)).ToArray();
                return dbContext.UserProfileDiscussions.Find(ids[1], ids[0]);
            }
        }
    }
}