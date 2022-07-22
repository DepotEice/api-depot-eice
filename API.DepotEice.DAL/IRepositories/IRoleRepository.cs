using API.DepotEice.DAL.Entities;

namespace API.DepotEice.DAL.IRepositories
{
    public interface IRoleRepository : IRepositoryBase<RoleEntity, string>
    {
        bool AddUser(string id, string userId);
        bool RemoveUser(string id, string userId);
        IEnumerable<RoleEntity> GetUserRoles(string userId);
    }
}
