using HackerCentral.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HackerCentral.ViewModels
{
    public class EmbeddedViewModel
    {
        public int conversationID { get; set; }
        public bool adminMode { get; set; }
        public string adminTitle { get; set; }
        public string normalTitle { get; set; }

        public EmbeddedViewModel()
        {
            conversationID = AthenaBridgeAPISettings.CONVERSATION_ID;
        }

        public string GetIFrameURL()
        {
            return "http://athenabridge.com/read/" + conversationID;
        }
    }
}