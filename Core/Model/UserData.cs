using System.Collections.Generic;

namespace Core.Model
{
    public class UserData
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public Language Language { get; set; }
        public List<UserCompany> UserCompanies { get; set; }
    }
}
