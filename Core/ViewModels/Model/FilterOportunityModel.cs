using Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.ViewModels.Model
{
    public class FilterOportunityModel
    {
        public DateTime dateFrom { get; set; }
        public DateTime dateTo { get; set; }
        public List<oppSta> status { get; set; }
        public List<cust> customers { get; set; }
        public List<prod> products { get; set; }
        public decimal? priceFrom { get; set; }
        public decimal? priceTo { get; set; }
        public List<comp> companies { get; set; }
        public int sellerId { get; set; }
    }

    public class cust
    {
        public int id { get; set; }
    }
    public class oppSta
    {
        public int id { get; set; }
    }
    public class prod
    {
        public int id { get; set; }
    }
    public class comp
    {
        public int id { get; set; }
    }
}
