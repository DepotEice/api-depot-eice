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
    public class ScheduleProfile : Profile
    {
        public ScheduleProfile()
        {
            // ScheduleEntity --> ScheduleModel

            CreateMap<ScheduleEntity, ScheduleDto>();

            // ScheduleModel --> ScheduleEntity

            CreateMap<ScheduleDto, ScheduleEntity>()
                .ForMember(
                    dest => dest.ModuleId,
                    opt => opt.MapFrom(src => src.Module.Id));
        }
    }
}
