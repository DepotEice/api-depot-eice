using API.DepotEice.BLL.Dtos;

namespace API.DepotEice.BLL.IServices
{
    public interface IScheduleService : IServiceBase<int, ScheduleDto>
    {
        ScheduleDto? CreateSchedule(int moduleId, ScheduleDto model);
        bool DeleteSchedule(int id);
        ScheduleDto? GetSchedule(int id);
        IEnumerable<ScheduleDto> GetModuleSchedules(int moduleId);
        ScheduleDto? UpdateSchedule(ScheduleDto model);
    }
}
