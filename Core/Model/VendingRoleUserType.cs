using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Model
{
    public class VendingRoleUserType
    {
        public int Id { get; set; }
        public int VendingUserTypeId { get; set; }
        public VendingUserType VendingUserType { get; set; }
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
    }
}
