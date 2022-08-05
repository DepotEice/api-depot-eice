using API.DepotEice.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL.IServices
{
    public interface IScheduleService
    {
        ScheduleModel? CreateSchedule(ScheduleModel model);
        bool DeleteSchedule(int id);
        ScheduleModel? GetSchedule(int id);
        IEnumerable<ScheduleModel> GetModuleSchedules(int moduleId);
        ScheduleModel? UpdateSchedule(ScheduleModel model);
    }
}
