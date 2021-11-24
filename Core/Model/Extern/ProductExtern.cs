using System;

namespace Core.Model.Extern
{
    [Serializable]
    public class ProductExtern
    {
        public int Id { get; set; }
        public string name { get; set; }
        public double price { get; set; }
        public int stock { get; set; }
        public int Discount { get; set; }
        public int quantity { get; set; }
    }
}
