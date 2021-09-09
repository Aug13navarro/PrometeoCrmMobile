using System.Collections.Generic;

namespace Core.Model
{
    public class Product
    {
        public int Id { get; set; }
        public string name { get; set; }
        public double price { get; set; }
        public List<PriceList> priceList { get; set; }
        public int stock { get; set; }
        public int Discount { get; set; }
        public int quantity { get; set; }
    }
}
