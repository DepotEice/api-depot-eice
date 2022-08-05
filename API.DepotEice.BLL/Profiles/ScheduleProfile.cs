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
    public class ScheduleProfile : Profile
    {
        public ScheduleProfile()
        {
            // ScheduleEntity --> ScheduleModel

            CreateMap<ScheduleEntity, ScheduleModel>();

            // ScheduleModel --> ScheduleEntity

            CreateMap<ScheduleModel, ScheduleEntity>()
                .ForMember(
                    dest => dest.ModuleId,
                    opt => opt.MapFrom(src => src.Module.Id));
        }
    }
}
