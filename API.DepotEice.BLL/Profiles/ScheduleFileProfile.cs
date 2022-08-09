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
    public class ScheduleFileProfile : Profile
    {
        public ScheduleFileProfile()
        {
            // ScheduleFileModel --> ScheduleFileEntity

            CreateMap<ScheduleFileData, ScheduleFileEntity>()
                .ForMember(
                    dest => dest.ScheduleId,
                    opt => opt.MapFrom(src => src.Schedule.Id));

            // ScheduleFileEntity --> ScheduleFileModel

            CreateMap<ScheduleFileEntity, ScheduleFileData>();
        }
    }
}
