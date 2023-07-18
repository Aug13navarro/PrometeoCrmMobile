using System;

namespace Core.Model
{
    public class User
    {
        public int? CompanyId { get; set; }
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public int? IdUser { get; set; }
        public DateTime? Expiration { get; set; }
        public Language Language { get; set; }

        public string RolesStr { get; set; }
        public string UniqueCompany { get; set; }
    }
}
