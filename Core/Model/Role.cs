﻿namespace Core.Model
{
    public class Role
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        //public IList<Permission> Permissions { get; set; }

        //public IList<PermissionDto> RolePermissions { get; set; }

    }
}
