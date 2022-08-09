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
    public class UserTokenProfile : Profile
    {
        public UserTokenProfile()
        {
            // UserTokenEntity --> UserTokenModel

            CreateMap<UserTokenEntity, UserTokenDto>();

            // UserTokenModel --> UserTokenEntity

            CreateMap<UserTokenDto, UserTokenEntity>()
                .ForMember(
                    dest => dest.UserId,
                    opt => opt.MapFrom(src => src.User.Id));
        }
    }
}
