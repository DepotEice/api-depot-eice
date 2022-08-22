using API.DepotEice.BLL.Dtos;

namespace API.DepotEice.BLL.IServices
{
    public interface IModuleService : IServiceBase<int, ModuleDto>
    {
        bool AddUser(int id, string userId, bool isAccepted);
        bool RemoveUser(int id, string userId);
        ModuleDto? CreateModule(ModuleDto model);
        ModuleDto? UpdateModule(ModuleDto model);
        bool DeleteModule(int id);
        IEnumerable<ModuleDto> GetModules();
        ModuleDto? GetModule(int id);
        IEnumerable<ModuleDto> GetUserModules(string userId);
        bool StudentApply(string sId, int mId);
        bool StudentAcceptExempt(string sId, int mId, bool decision);
        bool DeleteStudentFromModule(string sId, int mId);
        IEnumerable<UserDto> GetModuleStudents(int mId);
    }
}
