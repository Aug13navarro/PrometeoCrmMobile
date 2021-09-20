using System;

namespace Core.ViewModels.Model
{
    public class FilterOrderModel
    {
        public DateTime dateFrom { get; set; }
        public DateTime dateTo { get; set; }
        public int? companyId { get; set; }
        public int? orderStatusId { get; set; }
        public double? priceFrom { get; set; }
        public double? priceTo { get; set; }
    }
}
