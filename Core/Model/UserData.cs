using System.Collections.Generic;

namespace Core.Model
{
    public class UserData
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Language { get; set; }
        public IList<Role> Roles { get; set; }
    }
}
