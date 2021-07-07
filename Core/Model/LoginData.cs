using System;

namespace Core.Model
{
    public class LoginData
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
