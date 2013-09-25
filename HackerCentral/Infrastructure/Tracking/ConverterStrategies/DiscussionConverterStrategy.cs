using HackerCentral.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HackerCentral.Infrastructure.Tracking.ConverterStrategies
{
    public class DiscussionConverterStrategy : IConverterStrategy
    {
        public Object ConvertEntityTrack(EntityTrack entityTrack)
        {
            return FetchDiscussion(entityTrack);
        }

        public Dictionary<string, string> GetEntityValues(EntityTrack entityTrack)
        {
            var discussion = FetchDiscussion(entityTrack);
            var dictionary = new Dictionary<string, string>(4);

            dictionary.Add("API Key", discussion.ApiKey);
            dictionary.Add("Conversation ID", discussion.ConversationId.ToString());
            dictionary.Add("User Id", discussion.UserId.ToString());

            if (discussion.UserDiscussion == null)
            {
                dictionary.Add("User Discussions", "[null]");
            }
            else
            {
                var discussionIds = discussion.UserDiscussion.Select
                    <UserProfileDiscussions, string>(d => d.discussionId + "-" + d.userProfileId);
                var discussionString = String.Join(", ", discussionIds);
                dictionary.Add("User Discussions", discussionString);
            }

            return dictionary;
        }

        private Discussion FetchDiscussion(EntityTrack entityTrack)
        {
            using (SimpleContext dbContext = new HackerCentralContext(null))
            {
                int id = Int32.Parse(entityTrack.EntityId);
                return dbContext.Discussions.Find(id);
            }
        }
    }
}