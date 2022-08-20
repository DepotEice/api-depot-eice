using API.DepotEice.BLL.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL.IServices
{
    public interface IScheduleFileService
    {
        ScheduleFileDto? CreateScheduleFile(ScheduleFileDto model);
        bool DeleteScheduleFile(int id);
        ScheduleFileDto? GetScheduleFile(int id);
        IEnumerable<ScheduleFileDto> GetScheduleFiles(int scheduleId);
    }
}
