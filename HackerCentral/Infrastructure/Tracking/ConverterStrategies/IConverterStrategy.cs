using HackerCentral.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerCentral.Infrastructure.Tracking.ConverterStrategies
{
    public interface IConverterStrategy
    {
        public Object ConvertEntityTrack(EntityTrack entityTrack);
        public Dictionary<string, string> GetEntityValues(EntityTrack entityTrack);
    }
}
