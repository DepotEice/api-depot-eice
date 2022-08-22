using API.DepotEice.BLL.Dtos;
using API.DepotEice.DAL.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL.Profiles
{
    public class ModuleProfile : Profile
    {
        public ModuleProfile()
        {
            // ModuleEntity --> ModuleModel

            CreateMap<ModuleEntity, ModuleDto>();

            // ModuleModel --> ModuleEntity

            CreateMap<ModuleDto, ModuleEntity>();
        }
    }
}
