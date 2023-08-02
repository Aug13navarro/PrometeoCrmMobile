using SQLite;

namespace Core.Data.Tables
{
    [Table("Company")]
    public class CompanyTable
    {
        public int Id { get; set; }
        public string BusinessName { get; set; }
        public int? ExternalId { get; set; }
        public int? externalErpId { get; set; }
        public bool? ExportPv { get; set; }
    }
}
