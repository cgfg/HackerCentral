using HackerCentral.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace HackerCentral.Accessors
{
    public class PointVersionAccessor
    {
        private int conversationId;
        private string apiKey;
        private int userId;

        public PointVersionAccessor()
        {
            conversationId = AthenaBridgeAPISettings.CONVERSATION_ID;
            apiKey = AthenaBridgeAPISettings.API_KEY;
            userId = AthenaBridgeAPISettings.USER_ID;
        }

        public List<PointVersion> GetAllPointVersions()
        {
            string api_url = String.Format("http://129.93.238.144/api/{0}/{1}/point_versions", apiKey, conversationId);

            using (var w = new WebClient())
            {
                try
                {
                    var jsonData = string.Empty;
                    jsonData = w.DownloadString(api_url);
                    List<PointVersion> versions = JsonConvert.DeserializeObject<List<PointVersion>>(jsonData);
                    if (versions.Count > 0)
                        return versions;
                    else
                        return new List<PointVersion>();
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        public List<PointVersion> GetPointVersionsByPoint(long pointId)
        {
            string api_url = String.Format("http://129.93.238.144/api/{0}/{1}/point_versions/point/{2}", apiKey, conversationId, pointId);

            using (var w = new WebClient())
            {
                try
                {
                    var jsonData = string.Empty;
                    jsonData = w.DownloadString(api_url);
                    List<PointVersion> versions = JsonConvert.DeserializeObject<List<PointVersion>>(jsonData);
                    if (versions.Count > 0)
                        return versions;
                    else
                        return new List<PointVersion>();
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }
    }
}