using SQLite;

namespace Core.Data.Tables
{
    [Table("TransportCompany")]
    public class TransportCompanyTable
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string TransportName { get; set; }
    }
}
