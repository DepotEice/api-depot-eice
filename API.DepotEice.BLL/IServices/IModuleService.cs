using API.DepotEice.BLL.Models;

namespace API.DepotEice.BLL.IServices
{
    public interface IModuleService : IServiceBase<int, ModuleData>
    {
        bool AddUser(int id, string userId, bool isAccepted);
        bool RemoveUser(int id, string userId);
        ModuleData? CreateModule(ModuleData model);
        ModuleData? UpdateModule(ModuleData model);
        bool DeleteModule(int id);
        IEnumerable<ModuleData> GetModules();
        ModuleData? GetModule(int id);
        IEnumerable<ModuleData> GetUserModules(string userId);
    }
}
