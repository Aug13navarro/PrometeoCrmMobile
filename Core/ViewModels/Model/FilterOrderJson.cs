﻿using Core.Model;
using System;

namespace Core.ViewModels.Model
{
    public class FilterOrderJson
    {
        public DateTime dateFrom { get; set; }
        public DateTime dateTo { get; set; }
        public OpportunityStatus status { get; set; }
        public Company company { get; set; }
        public decimal? priceFrom { get; set; }
        public decimal? priceTo { get; set; }
    }
}
