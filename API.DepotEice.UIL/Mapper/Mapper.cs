using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using DevHopTools.Extensions;

namespace API.DepotEice.UIL.Mapper
{
    internal static class Mapper
    {
        internal static ModuleModel ToUil(this BLL.Models.ModuleModel data)  => data.Map<ModuleModel>();
        internal static BLL.Models.ModuleModel ToBll(this ModuleForm form) => form.Map<BLL.Models.ModuleModel>();
    }
}
