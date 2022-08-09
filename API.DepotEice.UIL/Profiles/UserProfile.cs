using API.DepotEice.UIL.Models;
using AutoMapper;
using BLL = API.DepotEice.BLL;


namespace API.DepotEice.UIL.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserDto, UserModel>();
        }
    }
}
