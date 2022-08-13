using API.DepotEice.BLL.Models;

namespace API.DepotEice.BLL.IServices
{
    public interface IModuleService : IServiceBase<int, ModuleData>
    {
        bool AddUser(int id, string userId, bool isAccepted);
        IEnumerable<UserModel> GetModuleStudents(int moduleId);
        bool DeleteModule(int id);
        bool StudentApply(string userId, int moduleId);
        bool StudentAcceptExempt(string userId, int moduleId, bool decision);
        bool DeleteStudentFromModule(string userId, int moduleId);
        bool RemoveUser(int id, string userId);
        ModuleData? CreateModule(ModuleData model);
        ModuleData? UpdateModule(ModuleData model);
        IEnumerable<ModuleData> GetModules();
        ModuleData? GetModule(int id);
        IEnumerable<ModuleData> GetUserModules(string userId);
    }
}
