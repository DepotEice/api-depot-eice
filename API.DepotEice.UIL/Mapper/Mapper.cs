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
    }
}
