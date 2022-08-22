using API.DepotEice.BLL.Dtos;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using AutoMapper;

namespace API.DepotEice.UIL.Profiles
{
    public class ModuleProfile : Profile
    {
        public ModuleProfile()
        {
            CreateMap<ModuleDto, ModuleModel>();
            CreateMap<ModuleForm, ModuleDto>();
        }
    }
}
