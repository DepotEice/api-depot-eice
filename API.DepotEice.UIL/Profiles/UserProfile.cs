using API.DepotEice.BLL.Dtos;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using AutoMapper;
using API.DepotEice.BLL;


namespace API.DepotEice.UIL.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserDto, UserModel>();

            CreateMap<RegisterForm, UserDto>();
        }
    }
}
