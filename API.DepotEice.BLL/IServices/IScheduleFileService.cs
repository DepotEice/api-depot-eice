using API.DepotEice.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL.IServices
{
    public interface IScheduleFileService
    {
        ScheduleFileData? CreateScheduleFile(int scheduleId, ScheduleFileData model);
        bool DeleteScheduleFile(int id);
        ScheduleFileData? GetScheduleFile(int id);
        IEnumerable<ScheduleFileData> GetScheduleFiles(int scheduleId);
    }
}
