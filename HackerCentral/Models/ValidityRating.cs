using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HackerCentral.Models
{
    public class ValidityRating
    {
        public long? conversation_id { get; set; }
        public DateTime? created_at { get; set; }
        public long? id { get; set; }
        public long? initial_statement_id { get; set; }
        public long? point_id { get; set; }
        public Side? side { get; set; }
        public DateTime? updated_at { get; set; }
        public long? user_credibility { get; set; }
        public long? user_id { get; set; }
        public long? validity_rating { get; set; }


        public enum Side { pro, con };
    }

}
