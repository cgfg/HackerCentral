using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace HackerCentral.Models
{
    public class PointVersion
    {
        public Category? category {get; set;}
        public bool? collaboratable {get; set;}
        public long? conversation_depth {get; set;}
        public long? conversation_id {get; set;}
        public long? counterpoint_count {get; set;}
        public DateTime? created_at {get; set;}
        public bool? editable {get; set;}
        public string full_text {get; set;}
        public long? id {get; set;}
        public long? initial_statement_id {get; set;}
        public bool? is_hidden {get; set;}
        public long? parent_id {get; set;}
        public long? point_id {get; set;}
        public long? point_version {get; set;}
        public double? quality {get; set;}
        public long? relevance_ratings_count {get; set;}
        public double? relevance_score {get; set;}
        public long? response_count {get; set;}
        public Side? side {get; set;}
        public bool? spam {get; set;}
        public bool? spam_child {get; set;}
        public string summary {get; set;}
        public DateTime? updated_at {get; set;}
        public long? user_id {get; set;}
        public long? validity_ratings_count {get; set;}
        public double? validity_score {get; set;}
        public long? views { get; set; }

        public enum Side { pro, con };
        public enum Category {
            [Description("Supporting")]
            SUPPORTING = 1,
            [Description("Opposing")]
            OPPOSING = 2 }
    }
}