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
        RoleModel? CreateRole(RoleModel model);
        bool DeleteRole(string id);
        IEnumerable<RoleModel> GetRoles();
        IEnumerable<RoleModel> GetUserRoles(string userId);
        RoleModel? GetRole(string id);
        RoleModel? UpdateRole(RoleModel model);
    }
}
