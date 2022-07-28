using API.DepotEice.BLL.Models;
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

            CreateMap<ModuleEntity, ModuleModel>();

            // ModuleModel --> ModuleEntity

            CreateMap<ModuleModel, ModuleEntity>();
        }
    }
}
