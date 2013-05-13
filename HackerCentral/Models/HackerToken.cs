using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HackerCentral.Models
{
    [Table("HackerToken")]
    public class HackerToken
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int TokenId { get; set; }
        public string Value { get; set; }
        public virtual UserProfile Consumer { get; set; }
    }
}