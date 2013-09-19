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
            var dictionary = new Dictionary<string, string>();

            dictionary.Add("AuthProvider", userProfile.AuthProvider.ToString());
            dictionary.Add("FullName", userProfile.FullName);
            dictionary.Add("UserId", userProfile.UserId.ToString());
            
            var tokenIds = userProfile.RegisteredTokens.Select<HackerToken, int>(token => token.Id);
            var tokenString = tokenIds.Join(", ");

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
