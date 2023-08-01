using API.DepotEice.DAL.Entities;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using AutoMapper;

namespace API.DepotEice.UIL.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserEntity, UserModel>();

            CreateMap<UserEntity, UserRequestingModuleModel>();

            CreateMap<UserForm, UserEntity>();
        }
    }
}
