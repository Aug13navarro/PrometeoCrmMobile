using SQLite;
using System;

namespace Core.Data.Tables
{
    [Table("AssistantComercial")]
    public class AssistantComercialTable
    {
        public int? CompanyId { get; set; }
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public int? IdUser { get; set; }
        public DateTime? Expiration { get; set; }

        public string RolesStr { get; set; }
        public string UniqueCompany { get; set; }
    }
}
