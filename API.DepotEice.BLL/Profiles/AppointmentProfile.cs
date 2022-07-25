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
    public class AppointmentProfile : Profile
    {
        public AppointmentProfile()
        {
            // AppointmentModel -> AppointmentEntity

            CreateMap<AppointmentModel, AppointmentEntity>()
                .ForMember(
                    dest => dest.UserId,
                    opt => opt.MapFrom(src => src.User.Id));

            // AppointmentEntity -> AppointmentModel

            CreateMap<AppointmentEntity, AppointmentModel>();
        }
    }
}
