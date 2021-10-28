using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Model.Extern
{
    [Serializable]
    public class OpportunityProductsExtern
    {
        public int productId { get; set; }
        public ProductExtern product { get; set; }

        public int Quantity { get; set; }

        public int Discount { get; set; }

        public double Price { get; set; }

        public double Total { get; set; }

        //para pasar company a producto
        public int CompanyId { get; set; }
    }
}
