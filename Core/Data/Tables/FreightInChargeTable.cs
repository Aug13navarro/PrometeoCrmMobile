using SQLite;

namespace Core.Data.Tables
{
    [Table("FreightInCharge")]
    public class FreightInChargeTable
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
