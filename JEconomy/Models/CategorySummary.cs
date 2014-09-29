using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JEconomy.Models
{
    public class CategorySummary
    {
        public string Category { get; set; }
        public double Value { get; set; }
        public List<SubCategorySummary> SubCategorySummaries { get; set; }
    }
}
