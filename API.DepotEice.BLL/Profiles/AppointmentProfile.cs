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
            CreateMap<AppointmentEntity, AppointmentModel>();
            CreateMap<UserModel, AppointmentModel>()
                .ForMember(
                    dest => dest.User,
                    opt => opt.MapFrom(src => src));
        }
    }
}
