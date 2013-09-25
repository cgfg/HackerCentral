using HackerCentral.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HackerCentral.Infrastructure.Tracking
{
    /// <summary>
    /// This class takes EntityTracks and returns the corresponding entities from the database.
    /// In essence, it "converts" between EntityTracks and the objects they point at.
    /// </summary>
    /// <remarks>
    /// This class uses the strategy and factory patterns to work. When it starts processing an
    /// EntityTrack, it uses a factory to pick out an IConvertStrategy object to handle the actual
    /// conversion process.
    /// </remarks>
    public class EntityConverter
    {
        public Object ConvertEntityTrack(EntityTrack entityTrack)
        {
            if (entityTrack.TimeRemoved.HasValue)
            {
                return (Object) null;
            }
            else
            {
                var factory = new ConverterStrategyFactory();
                var converterStrategy = factory.GetConverterStrategy(entityTrack);
                return converterStrategy.ConvertEntityTrack(entityTrack);
            }
        }

        public Dictionary<string, string> GetEntityValues(EntityTrack entityTrack)
        {
            if (entityTrack.TimeRemoved.HasValue)
            {
                return new Dictionary<string, string>();
            }
            else
            {
                var factory = new ConverterStrategyFactory();
                var converterStrategy = factory.GetConverterStrategy(entityTrack);
                return converterStrategy.GetEntityValues(entityTrack);
            }
        }
    }
}