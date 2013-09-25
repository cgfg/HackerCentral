using HackerCentral.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HackerCentral.Infrastructure.Tracking.ConverterStrategies
{
    public class HackerTokenConverterStrategy : IConverterStrategy
    {
        public Object ConvertEntityTrack(EntityTrack entityTrack)
        {
            return FetchHackerToken(entityTrack);
        }

        public Dictionary<string, string> GetEntityValues(EntityTrack entityTrack)
        {
            var hackerToken = FetchHackerToken(entityTrack);
            var dictionary = new Dictionary<string, string>(3);

            dictionary.Add("ID", hackerToken.Id.ToString());
            dictionary.Add("Value", hackerToken.Value);

            if (hackerToken.Consumers == null)
            {
                dictionary.Add("Consumers", "[null]");
            }
            else
            {
                var discussionIds = hackerToken.Consumers.Select
                    <UserProfile, int>(u => u.UserId);
                var discussionString = String.Join(", ", discussionIds);
                dictionary.Add("Consumers", discussionString);
            }

            return dictionary;
        }

        private HackerToken FetchHackerToken(EntityTrack entityTrack)
        {
            using (SimpleContext dbContext = new HackerCentralContext(null))
            {
                int id = Int32.Parse(entityTrack.EntityId);
                return dbContext.HackerTokens.Find(id);
            }
        }
    }
}