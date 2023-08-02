using SQLite;

namespace Core.Data.Tables
{
    [Table("Provider")]
    public class ProviderTable
    {
        public int Id { get; set; }
        public bool? Active { get; set; }
        public string Name { get; set; }
        public string BusinessName { get; set; }
        public int? IdCompany { get; set; }
    }
}
