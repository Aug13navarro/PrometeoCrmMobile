using SQLite;

namespace Core.Model
{
    public class TransportCompany
    {
        public int Id { get; set; }
        public string TransportName { get; set; }
        public int CompanyId { get; set; }
    }
}
