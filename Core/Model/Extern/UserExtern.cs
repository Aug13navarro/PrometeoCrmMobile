using System;

namespace Core.Model.Extern
{
    [Serializable]
    public class UserExtern
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public DateTime? Expiration { get; set; }
        public string Language { get; set; }

        public string RolesStr { get; set; }
    }
}
