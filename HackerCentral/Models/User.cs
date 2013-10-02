using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HackerCentral.Models
{
    public class User
    {
        public long? id { get; set; }
        public string username { get; set; }
        public long? highly_rated_points_count { get; set; }
        public long? count_of_average_points_by_participant { get; set; }
        public long? lowly_rated_points_count { get; set; }
        public double? local_credibility { get; set; }
        public double? global_credibility { get; set; }
        public long? all_ratings_by_participant_count { get; set; }

        // Assume data comes in this format: 64,"Lucas Cioffi",0,3,0,1.0,1.0,0
        public User(string data)
        {
            string[] members = data.Split(',');

            id = long.Parse(members[0]);
            username = members[1];
            highly_rated_points_count = long.Parse(members[2]);
            count_of_average_points_by_participant = long.Parse(members[3]);
            lowly_rated_points_count = long.Parse(members[4]);
            local_credibility = double.Parse(members[5]);
            global_credibility = double.Parse(members[6]);
            all_ratings_by_participant_count = long.Parse(members[7]);
        }
    }
}