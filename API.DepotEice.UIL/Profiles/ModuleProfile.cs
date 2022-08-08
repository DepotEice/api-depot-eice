using API.DepotEice.BLL.Models;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using AutoMapper;

namespace API.DepotEice.UIL.Profiles
{
    public class ModuleProfile : Profile
    {
        public ModuleProfile()
        {
            CreateMap<ModuleData, ModuleModel>();
            CreateMap<ModuleForm, ModuleData>();
        }
    }
}
