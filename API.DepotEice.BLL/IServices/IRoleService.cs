using API.DepotEice.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL.IServices
{
    public interface IRoleService
    {
        bool AddUser(string id, string userId);
        bool RemoveUser(string id, string userId);
        RoleDto? CreateRole(RoleDto model);
        bool DeleteRole(string id);
        IEnumerable<RoleDto> GetRoles();
        IEnumerable<RoleDto> GetUserRoles(string userId);
        RoleDto? GetRole(string id);
        RoleDto? UpdateRole(RoleDto model);
        RoleDto? GetRoleByName(string roleName);
    }
}
