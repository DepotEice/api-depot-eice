using API.DepotEice.DAL.Entities;

namespace API.DepotEice.DAL.IRepositories;

public interface IModuleRepository : IRepositoryBase<int, ModuleEntity>
{
    IEnumerable<UserEntity> GetModuleStudents(int moduleId);
    IEnumerable<ModuleEntity> GetUserModules(string userId);
    bool StudentApply(string studentId, int moduleId);
    bool StudentAcceptExempt(string userId, int moduleId, bool decision);
    bool DeleteUserFromModule(string userId, int moduleId);
}
