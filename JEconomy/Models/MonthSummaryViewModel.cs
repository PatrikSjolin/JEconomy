using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JEconomy.Models
{
    public class MonthSummaryViewModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
        public List<CategorySummary> CategorySummaries {get; set; }
    }
}