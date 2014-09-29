using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JEconomy.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        //public virtual AspNetUser AspUser {get; set; }
    }
}