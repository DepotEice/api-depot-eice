using API.DepotEice.UIL.Models;
using BLL = API.DepotEice.BLL;
using AutoMapper;
using API.DepotEice.UIL.Models.Forms;

namespace API.DepotEice.UIL.Profiles
{
    public class ModuleProfile : Profile
    {
        public ModuleProfile()
        {
            CreateMap<BLL.Models.ModuleModel, ModuleModel>();
            CreateMap<ModuleForm, BLL.Models.ModuleModel>();
        }
    }
}
