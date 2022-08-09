using API.DepotEice.BLL.Models;

namespace API.DepotEice.BLL.IServices
{
    public interface IScheduleService : IServiceBase<int, ScheduleData>
    {
        ScheduleData? CreateSchedule(int moduleId, ScheduleData model);
        bool DeleteSchedule(int id);
        ScheduleData? GetSchedule(int id);
        IEnumerable<ScheduleData> GetModuleSchedules(int moduleId);
        ScheduleData? UpdateSchedule(ScheduleData model);
    }
}
