using API.DepotEice.BLL.Dtos;

namespace API.DepotEice.BLL.IServices
{
    public interface IScheduleFileService
    {
        ScheduleFileDto? CreateScheduleFile(ScheduleFileDto model);
        ScheduleFileDto? CreateScheduleFile(int sId, ScheduleFileDto scheduleFileDto);
        bool DeleteScheduleFile(int id);
        ScheduleFileDto? GetScheduleFile(int id);
        IEnumerable<ScheduleFileDto> GetScheduleFiles(int scheduleId);
    }
}
