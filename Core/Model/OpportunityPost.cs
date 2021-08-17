using System;
using System.Collections.Generic;

namespace Core.Model
{
    public class OpportunityPost
    {
        public int customerId { get; set; }
        public int branchOfficeId { get; set; }
        public int opportunityStatusId { get; set; }
        public List<ProductSend> opportunityProducts { get; set; }
        public double totalPrice { get; set; }
        public string closedReason { get; set; }
        public DateTime closedDate { get; set; }
        public DateTime createDt { get; set; }
        public string description { get; set; }

        public class ProductSend
        {
            public int productId { get; set; }
            public double price { get; set; }
            public int quantity { get; set; }
            public int discount { get; set; }
            public double total { get; set; }
        }
    }
}
