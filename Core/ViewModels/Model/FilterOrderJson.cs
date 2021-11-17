using Core.Model;
using System;

namespace Core.ViewModels.Model
{
    public class FilterOrderJson
    {
        public DateTime dateFrom { get; set; }
        public DateTime dateTo { get; set; }
        public OpportunityStatus status { get; set; }
        public Company company { get; set; }
        public User Seller { get; set; }
        public double? priceFrom { get; set; }
        public double? priceTo { get; set; }
    }
}
