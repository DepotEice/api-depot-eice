using API.DepotEice.DAL.Entities;
using API.DepotEice.UIL.Models;
using AutoMapper;

namespace API.DepotEice.UIL.Profiles
{
    public class UserTokenProfile : Profile
    {
        public UserTokenProfile()
        {
            CreateMap<UserTokenModel, UserTokenEntity>();
        }
    }
}
