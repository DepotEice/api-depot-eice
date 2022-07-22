using API.DepotEice.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.DAL.IRepositories
{
    public interface IScheduleFileRepository : IRepositoryBase<ScheduleFileEntity, int>
    {
        /// <summary>
        /// Retrieve all files associated with a <see cref="ScheduleEntity"/>
        /// </summary>
        /// <param name="scheduleId"><see cref="ScheduleEntity"/>'s <see cref="ScheduleEntity.Id"/> 
        /// value</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="ScheduleFileEntity"/></returns>
        IEnumerable<ScheduleFileEntity> GetScheduleFiles(int scheduleId);
    }
}
