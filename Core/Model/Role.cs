﻿using System.Collections.Generic;

namespace Core.Model
{
    public class Role
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public List<VendingRoleUserType> VendingRoleUserTypes { get; set; }
    }
}
