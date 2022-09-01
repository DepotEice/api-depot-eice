using API.DepotEice.DAL.Entities;

namespace API.DepotEice.DAL.IRepositories;

public interface IRoleRepository : IRepositoryBase<string, RoleEntity>
{
    IEnumerable<RoleEntity> GetUserRoles(string userId);
    bool AddUser(string roleId, string userId);
    bool RemoveUser(string roleId, string userId);
    RoleEntity GetByName(string name);
}
