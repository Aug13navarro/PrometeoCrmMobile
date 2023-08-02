using SQLite;

namespace Core.Data.Tables
{
    [Table("PaymentCondition")]
    public class PaymentConditionTable
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int Code { get; set; }
        public int CompanyId { get; set; }
        public string Abbreviature { get; set; }
        public int Surcharge { get; set; }
        public bool Active { get; set; }
    }
}
