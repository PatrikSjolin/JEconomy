using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations;

namespace JEconomy.Models
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; }
        public virtual IdentityUser IdentityUser { get; set; }
        public string Name { get; set; }
    }
}