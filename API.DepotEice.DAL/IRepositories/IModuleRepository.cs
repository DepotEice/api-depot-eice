using API.DepotEice.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.DAL.IRepositories
{
    public interface IModuleRepository : IRepositoryBase<ModuleEntity, int>
    {
        bool AddUser(int id, string userId, bool isAccepted);
        bool RemoveUser(int id, string userId);
        IEnumerable<ModuleEntity> GetUserModules(string userId);
    }
}
