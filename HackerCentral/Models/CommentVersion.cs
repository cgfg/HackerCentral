using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HackerCentral.Models
{
    public class CommentVersion
    {
        public long? comment_id { get; set; }
        public long? comment_version { get; set; }
        public long? conversation_id { get; set; }
        public string full_text { get; set; }
        public long? id { get; set; }
        public long? point_id { get; set; }
        public DateTime updated_at { get; set; }
        public DateTime created_at { get; set; }
        public long? user_id { get; set; }
    }
}