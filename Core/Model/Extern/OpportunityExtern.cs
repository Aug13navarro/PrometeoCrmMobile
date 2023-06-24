using System;
using System.Collections.Generic;

namespace Core.Model.Extern
{
    [Serializable]
    public class OpportunityExtern
    {
        public int Id { get; set; }

        public CustomerExtern customer { get; set; }
        public CompanyExtern company { get; set; }

        public OpportunityStatusExtern opportunityStatus { get; set; }

        public DateTime createDt { get; set; }
        public DateTime closedDate { get; set; }

        public List<OpportunityProductsExtern> opportunityProducts { get; set; } = new List<OpportunityProductsExtern>();

        public string description { get; set; }

        public decimal totalPrice { get; set; }
    }
}
