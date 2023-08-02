using SQLite;

namespace Core.Data.Tables
{
    [Table("Incoterm")]
    public class IncotermTable
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string ExternalId { get; set; }
    }
}
