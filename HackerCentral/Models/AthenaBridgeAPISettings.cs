using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace HackerCentral.Models
{
    public static class AthenaBridgeAPISettings
    {
        public static readonly string API_KEY = ConfigurationManager.AppSettings["ApiKey"];
        public static readonly int CONVERSATION_ID = int.Parse(ConfigurationManager.AppSettings["ConversationId"]);
        public static readonly int USER_ID = int.Parse(ConfigurationManager.AppSettings["UserId"]);
    }
}