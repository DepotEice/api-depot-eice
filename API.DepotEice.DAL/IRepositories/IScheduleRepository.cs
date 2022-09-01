using API.DepotEice.DAL.Entities;

namespace API.DepotEice.DAL.IRepositories;

public interface IScheduleRepository : IRepositoryBase<int, ScheduleEntity>
{
    IEnumerable<ScheduleEntity> GetModuleSchedules(int moduleId);
}
