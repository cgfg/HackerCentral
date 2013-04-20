using HackerCentral.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace HackerCentral.Accessors
{
    public class UserAccessor
    {
        private int conversationId;
        private string apiKey;
        private int userId;

        public UserAccessor()
        {
            conversationId = AthenaBridgeAPISettings.CONVERSATION_ID;
            apiKey = AthenaBridgeAPISettings.API_KEY;
            userId = AthenaBridgeAPISettings.USER_ID;
        }

        // TODO: Test this...API seems to be down
        public List<User> GetAllUsers()
        {
            string api_url = String.Format("http://athenabridge.com/api/{0}/{1}/leaderboard", apiKey, conversationId);

            using (var w = new WebClient())
            {
                try
                {
                    var data = string.Empty;
                    data = w.DownloadString(api_url);

                    // [[64,"Lucas Cioffi",0,3,0,1.0,1.0,0],[3,"Joe Smith",0,1,0,1.0,1.0,0]]
                    data = data.Replace("[[", "").Replace("]]", "");
                    string[] userData = data.Split(new string[] { "],[" }, StringSplitOptions.RemoveEmptyEntries);

                    var users = new List<User>();
                    foreach (string s in userData)
                    {
                        users.Add(new User(s));
                    }

                    return users;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        public User GetUser(long id)
        {
            var users = GetAllUsers();
            try
            {
                return users.SingleOrDefault(m => m.id == id);
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}