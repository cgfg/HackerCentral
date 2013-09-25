using HackerCentral.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HackerCentral.Infrastructure.Tracking.ConverterStrategies
{
    public class DeliveryConverterStrategy : IConverterStrategy
    {
        public Object ConvertEntityTrack(EntityTrack entityTrack)
        {
            return FetchDelivery(entityTrack);
        }

        public Dictionary<string, string> GetEntityValues(EntityTrack entityTrack)
        {
            var delivery = FetchDelivery(entityTrack);
            var dictionary = new Dictionary<string, string>(5);

            dictionary.Add("ID", delivery.Id.ToString());

            if(delivery.Message == null)
            {
                dictionary.Add("Message", "[null]");
            }
            else
            {
                dictionary.Add("Message", delivery.Message.Id.ToString());
            }

            if (delivery.Reciever == null)
            {
                dictionary.Add("Reciever", "[null]");
            }
            else
            {
                dictionary.Add("Reciever", delivery.Reciever.UserId.ToString());
            }

            if (!delivery.TimeDelivered.HasValue)
            {
                dictionary.Add("Time Delivered", "[null]");
                dictionary.Add("Date Delivered", "[n/a]");
            }
            else
            {
                var time = delivery.TimeDelivered.Value;
                dictionary.Add("Time Delivered", time.ToShortTimeString());
                dictionary.Add("Date Delivered", time.ToShortDateString());
            }

            return dictionary;
        }

        private Delivery FetchDelivery(EntityTrack entityTrack)
        {
            using (SimpleContext dbContext = new HackerCentralContext(null))
            {
                int id = Int32.Parse(entityTrack.EntityId);
                return dbContext.Deliveries.Find(id);
            }
        }
    }
}