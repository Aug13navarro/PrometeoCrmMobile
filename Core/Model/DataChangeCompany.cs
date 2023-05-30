using System;

namespace Core
{
    public class DataChangeCompany
    {
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public TokenResponse Token { get; set; }
        public string Device { get; set; }
    }
    public class TokenResponse
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public DateTime? Expiration { get; set; }
    }
}
