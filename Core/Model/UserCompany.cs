namespace Core.Model
{
    public class UserCompany
    {
        public int RoleId { get; set; }
        public Role Roles { get; set; }
        public int LanguageId { get; set; }
        public int? CoordinatorId { get; set; }
    }
}
