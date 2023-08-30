using SQLite;

namespace Core.Data.Tables
{
    [Table("Product")]
    public class ProductTable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Stock { get; set; }
        public int Discount { get; set; }
        public int Quantity { get; set; }
        public int? CompanyId { get; set; }
    }
}
