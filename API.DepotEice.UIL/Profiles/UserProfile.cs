using API.DepotEice.BLL.Models;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using AutoMapper;
using BLL = API.DepotEice.BLL;


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
