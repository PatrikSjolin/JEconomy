using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JEconomy.Models
{
    public class TransactionsViewModel
    {
        public List<TransactionViewModel> Transactions { get; set; }
        public TimeSpan TimeElapsed { get; set; }
    }
}