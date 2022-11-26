using API.DepotEice.DAL.Entities;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using AutoMapper;

namespace API.DepotEice.UIL.Profiles
{
    public class ModuleProfile : Profile
    {
        public ModuleProfile()
        {
            CreateMap<ModuleForm, ModuleEntity>();
            CreateMap<ModuleEntity, ModuleModel>();
        }
    }
}
