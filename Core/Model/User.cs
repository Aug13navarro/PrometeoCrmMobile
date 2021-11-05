using System;
using System.Collections.Generic;

namespace Core.Model
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public DateTime? Expiration { get; set; }
        public string Language { get; set; }

        public string Roles { get; set; } 

    }
}
