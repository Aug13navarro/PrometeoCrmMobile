using Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.ViewModels.Model
{
    public class FilterOpportunityJson
    {
        public DateTime dateFrom { get; set; }
        public DateTime dateTo { get; set; }
        public List<OpportunityStatus> status { get; set; }
        public List<Customer> customers { get; set; }
        public List<Product> products { get; set; }
        public decimal? priceFrom { get; set; }
        public decimal? priceTo { get; set; }
        public List<Company> companies { get; set; }
    }
}
