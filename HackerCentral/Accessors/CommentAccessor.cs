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
    public class CommentAccessor
    {
        private int conversationId;
        private string apiKey;
        private int userId;

        public CommentAccessor()
        {
            conversationId = AthenaBridgeAPISettings.CONVERSATION_ID;
            apiKey = AthenaBridgeAPISettings.API_KEY;
            userId = AthenaBridgeAPISettings.USER_ID;
        }

        public List<Comment> GetAllComments()
        {
            string api_url = String.Format("http://129.93.238.144/api/{0}/{1}/comments", apiKey, conversationId);

            using (var w = new WebClient())
            {
                try
                {
                    var jsonData = string.Empty;
                    jsonData = w.DownloadString(api_url);
                    List<Comment> comments = JsonConvert.DeserializeObject<List<Comment>>(jsonData);
                    if (comments.Count > 0)
                        return comments;
                    else
                        return new List<Comment>();
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        public Comment GetComment(long id)
        {
            string api_url = String.Format("http://129.93.238.144/api/{0}/{1}/comments/{2}/show", apiKey, conversationId, id);

            using (var w = new WebClient())
            {
                try
                {
                    var jsonData = w.DownloadString(api_url);
                    Comment comment = JsonConvert.DeserializeObject<Comment>(jsonData);
                    return comment;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        public List<Comment> GetCommentsByPoint(long pointID)
        {
            string api_url = String.Format("http://129.93.238.144/api/{0}/{1}/comments/point/{2}", apiKey, conversationId, pointID);

            using (var w = new WebClient())
            {
                try
                {
                    var jsonData = w.DownloadString(api_url);
                    List<Comment> comments = JsonConvert.DeserializeObject<List<Comment>>(jsonData);
                    return comments;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        public bool DestroyComment(long id)
        {
            string api_url = String.Format("http://129.93.238.144/api/{0}/{1}/comments/{2}/destroy", apiKey, conversationId, id);

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

        public bool UpdateComment(Comment update)
        {
            // Notice: API is the same for Create Comment
            string api_url = String.Format("http://129.93.238.144/api/{0}/{1}/comments/create", apiKey, conversationId);

            try
            {
                var client = new RestClient();
                var request = new RestRequest(api_url, Method.POST);
                request.AddParameter("user_id", userId);
                request.AddParameter("point_id", update.point_id);
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

        public bool CreateComment(Comment update)
        {
            string api_url = String.Format("http://129.93.238.144/api/{0}/{1}/comments/create", apiKey, conversationId);

            try
            {
                var client = new RestClient();
                var request = new RestRequest(api_url, Method.POST);
                request.AddParameter("user_id", userId);
                request.AddParameter("point_id", update.point_id);
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

        

        
    }
}