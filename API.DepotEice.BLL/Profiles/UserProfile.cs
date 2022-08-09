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
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            // UserEntity --> UserModel

            CreateMap<UserEntity, UserDto>();

            // UserModel --> AppointmentModel

            CreateMap<UserDto, AppointmentModel>()
                .ForMember(
                    dest => dest.User,
                    opt => opt.MapFrom(src => src));

            // UserModel --> ArticleCommentModel

            CreateMap<UserDto, ArticleCommentModel>()
                .ForMember(
                    dest => dest.User,
                    opt => opt.MapFrom(src => src));

            // UserModel --> ArticleModel

            CreateMap<UserDto, ArticleModel>()
                .ForMember(
                    dest => dest.User,
                    opt => opt.MapFrom(src => src));

            CreateMap<UserDto, UserEntity>();
        }
    }
}
