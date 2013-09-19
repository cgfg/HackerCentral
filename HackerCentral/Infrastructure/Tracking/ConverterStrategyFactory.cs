﻿using HackerCentral.Infrastructure.Tracking.ConverterStrategies;
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
            string type = entityTrack.EntityType;

            if (type == "UserProfile")
            {
                return new UserProfileConverterStrategy();
            }
            else
            {
                throw new NotImplementedException("There is no IConverterStrategy for an entity track with EntityType == " + entityType);
            }
        }
    }
}