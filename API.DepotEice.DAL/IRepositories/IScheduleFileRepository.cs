using API.DepotEice.DAL.Entities;

namespace API.DepotEice.DAL.IRepositories;

public interface IScheduleFileRepository : IRepositoryBase<int, ScheduleFileEntity>
{
    /// <summary>
    /// Retrieve all files associated with a <see cref="ScheduleEntity"/>
    /// </summary>
    /// <param name="scheduleId"><see cref="ScheduleEntity"/>'s <see cref="ScheduleEntity.Id"/> 
    /// value</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="ScheduleFileEntity"/></returns>
    IEnumerable<ScheduleFileEntity> GetScheduleFiles(int scheduleId);
}
