using HackerCentral.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace HackerCentral.Accessors
{
    public class CommentVersionAccessor
    {
        private int conversationId;
        private string apiKey;
        private int userId;

        public CommentVersionAccessor()
        {
            conversationId = AthenaBridgeAPISettings.CONVERSATION_ID;
            apiKey = AthenaBridgeAPISettings.API_KEY;
            userId = AthenaBridgeAPISettings.USER_ID;
        }

        public List<CommentVersion> GetAllCommentVersions()
        {
            string api_url = String.Format("http://129.93.238.144/api/{0}/{1}/comment_versions", apiKey, conversationId);

            using (var w = new WebClient())
            {
                try
                {
                    var jsonData = string.Empty;
                    jsonData = w.DownloadString(api_url);
                    List<CommentVersion> versions = JsonConvert.DeserializeObject<List<CommentVersion>>(jsonData);
                    if (versions.Count > 0)
                        return versions;
                    else
                        return new List<CommentVersion>();
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        public List<CommentVersion> GetCommentVersionsByPoint(long pointId)
        {
            string api_url = String.Format("http://129.93.238.144/api/{0}/{1}/comment_versions/point/{2}", apiKey, conversationId, pointId);

            using (var w = new WebClient())
            {
                try
                {
                    var jsonData = string.Empty;
                    jsonData = w.DownloadString(api_url);
                    List<CommentVersion> versions = JsonConvert.DeserializeObject<List<CommentVersion>>(jsonData);
                    if (versions.Count > 0)
                        return versions;
                    else
                        return new List<CommentVersion>();
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        public List<CommentVersion> GetCommentVersionsByComment(long commentId)
        {
            string api_url = String.Format("http://129.93.238.144/api/{0}/{1}/comment_versions/comment/{2}", apiKey, conversationId, commentId);

            using (var w = new WebClient())
            {
                try
                {
                    var jsonData = string.Empty;
                    jsonData = w.DownloadString(api_url);
                    List<CommentVersion> versions = JsonConvert.DeserializeObject<List<CommentVersion>>(jsonData);
                    if (versions.Count > 0)
                        return versions;
                    else
                        return new List<CommentVersion>();
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }
    }
}