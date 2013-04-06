using HackerCentral.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Newtonsoft.Json;
using System.Net.Http;
using System.IO;
using System.Text;
using RestSharp;

namespace HackerCentral.Accessors
{
    public class PointAccessor
    {
        private int conversationId;
        private string apiKey;
        private int userId;

        public PointAccessor()
        {
            conversationId = AthenaBridgeAPISettings.CONVERSATION_ID;
            apiKey = AthenaBridgeAPISettings.API_KEY;
            userId = AthenaBridgeAPISettings.USER_ID;
        }

        public List<Point> GetAllPoints()
        {
            string api_url = String.Format("http://athenabridge.com/api/{0}/{1}/points", apiKey, conversationId);

            using (var w = new WebClient())
            {
                try
                {
                    var jsonData = string.Empty;
                    jsonData = w.DownloadString(api_url);
                    List<Point> points = JsonConvert.DeserializeObject<List<Point>>(jsonData);
                    if (points.Count > 0)
                        return points;
                    else
                        return new List<Poitnt>();
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        public Point GetPoint(long id)
        {
            string api_url = String.Format("http://athenabridge.com/api/{0}/{1}/points/{2}/show", apiKey, conversationId, id);

            using (var w = new WebClient())
            {
                try
                {
                    var jsonData = w.DownloadString(api_url);
                    Point point = JsonConvert.DeserializeObject<Point>(jsonData);
                    return point;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        public bool DestroyPoint(long id)
        {
            string api_url = String.Format("http://athenabridge.com/api/{0}/{1}/points/{2}/destroy", apiKey, conversationId, id);

            try
            {
                var client = new RestClient();
                var request = new RestRequest(api_url);
                var response = client.Execute(request);
                var content = response.Content;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }

        public bool UpdatePoint(Point update)
        {
            string api_url = String.Format("http://athenabridge.com/api/{0}/{1}/points/{2}/update", apiKey, conversationId, update.id);

            try
            {
                var client = new RestClient();
                var request = new RestRequest(api_url, Method.POST);
                request.AddParameter("api_key", apiKey);
                request.AddParameter("user_ud", userId);
                request.AddParameter("category", update.category);
                request.AddParameter("summary", update.summary);
                request.AddParameter("full_text", update.full_text);
                var response = client.Execute(request);
                var content = response.Content;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool CreatePoint(Point create)
        {
            string api_url = String.Format("http://athenabridge.com/api/{0}/{1}/points/create", apiKey, conversationId);

            try
            {
                var client = new RestClient();
                var request = new RestRequest(api_url, Method.POST);
                request.AddParameter("api_key", apiKey);
                request.AddParameter("user_ud", userId);
                request.AddParameter("parent_id", create.parent_id);
                request.AddParameter("category", create.category);
                request.AddParameter("summary", create.summary);
                request.AddParameter("full_text", create.full_text);
                var response = client.Execute(request);
                var content = response.Content;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}