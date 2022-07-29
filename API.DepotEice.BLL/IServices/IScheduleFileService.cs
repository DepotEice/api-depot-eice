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
        ScheduleFileModel? CreateScheduleFile(ScheduleFileModel model);
        bool DeleteScheduleFile(int id);
        ScheduleFileModel? GetScheduleFile(int id);
        IEnumerable<ScheduleFileModel> GetScheduleFiles(int scheduleId);
    }
}
