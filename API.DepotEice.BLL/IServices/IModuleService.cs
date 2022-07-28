using API.DepotEice.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL.IServices
{
    public interface IModuleService
    {
        bool AddUser(int id, string userId, bool isAccepted);
        bool RemoveUser(int id, string userId);
        ModuleModel? CreateModule(ModuleModel model);
        ModuleModel? UpdateModule(ModuleModel model);
        bool DeleteModule(int id);
        IEnumerable<ModuleModel> GetModules();
        ModuleModel? GetModule(int id);
        IEnumerable<ModuleModel> GetUserModules(string userId);
    }
}
