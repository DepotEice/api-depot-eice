using API.DepotEice.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.DAL.IRepositories
{
    public interface IScheduleRepository : IRepositoryBase<ScheduleEntity, int>
    {
        /// <summary>
        /// Retrieve all <see cref="ScheduleEntity"/> from the database where 
        /// <see cref="ModuleEntity.Id"/> equals <paramref name="moduleId"/>
        /// </summary>
        /// <param name="moduleId">
        /// <see cref="ModuleEntity"/> <see cref="ModuleEntity.Id"/>'s value
        /// </param>
        /// <returns>
        /// A <see cref="IEnumerable{T}"/> of all <see cref="ScheduleEntity"/> retrieved 
        /// from the database
        /// </returns>
        IEnumerable<ScheduleEntity> GetModuleSchedules(int moduleId);
    }
}
