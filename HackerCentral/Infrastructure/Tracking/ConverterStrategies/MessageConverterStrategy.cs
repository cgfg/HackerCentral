using HackerCentral.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HackerCentral.Infrastructure.Tracking.ConverterStrategies
{
    public class MessageConverterStrategy : IConverterStrategy
    {
        public Object ConvertEntityTrack(EntityTrack entityTrack)
        {
            return FetchMessage(entityTrack);
        }

        public Dictionary<string, string> GetEntityValues(EntityTrack entityTrack)
        {
            var message = FetchMessage(entityTrack);
            var dictionary = new Dictionary<string, string>(7);

            dictionary.Add("ID", message.Id.ToString());
            dictionary.Add("Group ID", message.GroupId);
            dictionary.Add("Text", message.Text);

            if (message.TimeStamp == null)
            {
                dictionary.Add("Timestamp Time", "[null]");
                dictionary.Add("Timestamp Date", "[n/a]");
            }
            else
            {
                var timestamp = message.TimeStamp;
                dictionary.Add("Timestamp Time", timestamp.ToShortTimeString());
                dictionary.Add("Timestamp Date", timestamp.ToShortDateString());
            }

            if (message.Sender == null)
            {
                dictionary.Add("Sender", "[null]");
            }
            else
            {
                dictionary.Add("Sender", message.Sender.UserId.ToString());
            }

            if (message.Deliveries == null)
            {
                dictionary.Add("Deliveries", "[null]");
            }
            else
            {
                var deliveriesIds = message.Deliveries.Select<Delivery, int>(d => d.Id);
                var deliveriesString = String.Join(", ", deliveriesIds);
                dictionary.Add("Deliveries", deliveriesString);
            }

            return dictionary;
        }

        private Message FetchMessage(EntityTrack entityTrack)
        {
            using (SimpleContext dbContext = new HackerCentralContext(null))
            {
                int id = Int32.Parse(entityTrack.EntityId);
                return dbContext.Messages.Find(id);
            }
        }
    }
}