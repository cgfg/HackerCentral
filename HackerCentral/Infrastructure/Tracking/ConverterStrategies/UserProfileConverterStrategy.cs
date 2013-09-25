using HackerCentral.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerCentral.Infrastructure.Tracking.ConverterStrategies
{
    public class UserProfileConverterStrategy : IConverterStrategy
    {
        public Object ConvertEntityTrack(EntityTrack entityTrack)
        {
            return FetchUserProfile(entityTrack);
        }

        public Dictionary<string, string> GetEntityValues(EntityTrack entityTrack)
        {
            var userProfile = FetchUserProfile(entityTrack);
            var dictionary = new Dictionary<string, string>(6);

            dictionary.Add("Auth Provider", userProfile.AuthProvider.ToString());
            dictionary.Add("Full Name", userProfile.FullName);
            dictionary.Add("User Id", userProfile.UserId.ToString());
            dictionary.Add("User Name", userProfile.UserName);

            if (userProfile.RegisteredTokens == null)
            {
                dictionary.Add("Registered Tokens", "[null]");
            }
            else
            {
                var tokenIds = userProfile.RegisteredTokens.Select<HackerToken, int>(token => token.Id);
                var tokenString = String.Join(", ", tokenIds);
                dictionary.Add("Registered Tokens", tokenString);
            }

            if (userProfile.UserDiscussion == null)
            {
                dictionary.Add("User Discussions", "[null]");
            }
            else
            {
                var discussionIds = userProfile.UserDiscussion.Select
                    <UserProfileDiscussions, string>(d => d.discussionId + "-" + d.userProfileId);
                var discussionString = String.Join(", ", discussionIds);
                dictionary.Add("User Discussions", discussionString);
            }

            return dictionary;
        }

        private UserProfile FetchUserProfile(EntityTrack entityTrack)
        {
            using (SimpleContext dbContext = new HackerCentralContext(null))
            {
                int id = Int32.Parse(entityTrack.EntityId);
                return dbContext.UserProfiles.Find(id);
            }
        }
    }
}
