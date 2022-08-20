using API.DepotEice.DAL.Entities;

namespace API.DepotEice.DAL.IRepositories
{
    public interface IModuleRepository : IRepositoryBase<ModuleEntity, int>
    {
        bool AddUser(int id, string userId, bool isAccepted);
        bool RemoveUser(int id, string userId);
        IEnumerable<UserEntity> GetModuleStudents(int moduleId);
        bool StuddentApply(string studentId, int moduleId);
        bool StudentAcceptExempt(string userId, int moduleId, bool decision);
        bool DeleteStudentFromModule(string userId, int moduleId);
        IEnumerable<ModuleEntity> GetUserModules(string userId);
    }
}
