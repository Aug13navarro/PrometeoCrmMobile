using SQLite;

namespace Core.Data.Tables
{
    [Table("PaymentMethod")]
    public class PaymentMethodTable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? CompanyId { get; set; }
    }
}
