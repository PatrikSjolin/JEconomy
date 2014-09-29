using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations;

namespace JEconomy.Models
{
    public class Transaction
    {
        [Key]
        public Guid Id { get; set; }
        public virtual IdentityUser IdentityUser { get; set; }
        public DateTime TransactionDate { get; set; }
        public double Value { get; set; }
        public double Balance { get; set; }
        public string Place { get; set; }
        public virtual Category Category { get; set; }
    }
}