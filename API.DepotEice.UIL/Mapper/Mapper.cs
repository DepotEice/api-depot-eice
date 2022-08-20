using API.DepotEice.BLL.Models;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using DevHopTools.Extensions;

namespace API.DepotEice.UIL.Mapper
{
    internal static class Mapper
    {
        // Modules
        internal static ModuleModel ToUil(this ModuleData data)  => data.Map<ModuleModel>();
        internal static ModuleData ToBll(this ModuleForm form) => form.Map<ModuleData>();

        // Schedules
        internal static ScheduleModel ToUil(this ScheduleData data) => data.Map<ScheduleModel>();
        internal static ScheduleData ToBll(this ScheduleForm form) => form.Map<ScheduleData>();

        // Schedule Files
        internal static ScheduleFileModel ToUil(this ScheduleFileData data) => data.Map<ScheduleFileModel>();
        internal static ScheduleFileData ToBll(this ScheduleFileData data) => data.Map<ScheduleFileData>();
        internal static ScheduleFileData ToBll(this ScheduleFileModel model) => model.Map<ScheduleFileData>();

    }
}
