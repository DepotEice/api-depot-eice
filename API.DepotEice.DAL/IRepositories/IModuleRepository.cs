using API.DepotEice.DAL.Entities;

namespace API.DepotEice.DAL.IRepositories;

public interface IModuleRepository : IRepositoryBase<int, ModuleEntity>
{
    IEnumerable<UserEntity> GetModuleUsers(int moduleId);
    IEnumerable<UserEntity> GetModuleUsers(int moduleId, string role);
    IEnumerable<ModuleEntity> GetUserModules(string userId);
    bool AddUserToModule(string studentId, int moduleId);
    bool AcceptUser(string userId, int moduleId, bool decision);
    bool DeleteUserFromModule(string userId, int moduleId);
}
