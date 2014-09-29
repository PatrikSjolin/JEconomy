using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JEconomy.Models
{
    public class TransactionViewModel
    {
        public DateTime TransactionDate { get; set; }
        public double Value { get; set; }
        public double Balance { get; set; }
        public string Place { get; set; }
        public string State { get; set; }
        public string Category { get; set; }
    }
}