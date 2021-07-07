using Core.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Model
{
    public class Role
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public Company Company { get; set; }
        public string Name { get; set; }
        public IList<Permission> Permissions { get; set; }

        public IList<PermissionDto> RolePermissions { get; set; }

    }
}
