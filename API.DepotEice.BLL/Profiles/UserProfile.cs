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
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            // UserEntity --> UserModel

            CreateMap<UserEntity, UserDto>();

            // UserModel --> AppointmentModel

            CreateMap<UserDto, AppointmentDto>()
                .ForMember(
                    dest => dest.User,
                    opt => opt.MapFrom(src => src));

            // UserModel --> ArticleCommentModel

            CreateMap<UserDto, ArticleCommentDto>()
                .ForMember(
                    dest => dest.User,
                    opt => opt.MapFrom(src => src));

            // UserModel --> ArticleModel

            CreateMap<UserDto, ArticleDto>()
                .ForMember(
                    dest => dest.User,
                    opt => opt.MapFrom(src => src));
        }
    }
}
