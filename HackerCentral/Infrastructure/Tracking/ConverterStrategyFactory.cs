using HackerCentral.Infrastructure.Tracking.ConverterStrategies;
using HackerCentral.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HackerCentral.Infrastructure.Tracking
{
    public class ConverterStrategyFactory
    {
        public IConverterStrategy GetConverterStrategy(EntityTrack entityTrack)
        {
            return GetConverterStrategy(entityTrack.EntityType);
        }

        public IConverterStrategy GetConverterStrategy(String type)
        {
            if (type == "UserProfile")
            {
                return new UserProfileConverterStrategy();
            }
            else if (type == "Discussion")
            {
                return new DiscussionConverterStrategy();
            }
            else if (type == "UserProfileDiscussions")
            {
                return new UserProfileDiscussionsConverterStrategy();
            }
            else if (type == "HackerToken")
            {
                return new HackerTokenConverterStrategy();
            }
            else if (type == "Message")
            {
                return new MessageConverterStrategy();
            }
            else if (type == "Delivery")
            {
                return new DeliveryConverterStrategy();
            }
            else if (type == null)
            {
                throw new ArgumentNullException("Cannot pass a null argument to ConverterStrategyFactory.GetConverterStrategy()");
            }
            else
            {
                throw new NotImplementedException("There is no IConverterStrategy for an entity track with EntityType == " + type);
            }
        }
    }
}