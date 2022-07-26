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

            CreateMap<UserEntity, UserModel>();

            // UserModel --> AppointmentModel

            CreateMap<UserModel, AppointmentModel>()
                .ForMember(
                    dest => dest.User,
                    opt => opt.MapFrom(src => src));

            // UserModel --> ArticleCommentModel

            CreateMap<UserModel, ArticleCommentModel>()
                .ForMember(
                    dest => dest.User,
                    opt => opt.MapFrom(src => src));

            // UserModel --> ArticleModel

            CreateMap<UserModel, ArticleModel>()
                .ForMember(
                    dest => dest.User,
                    opt => opt.MapFrom(src => src));
        }
    }
}
