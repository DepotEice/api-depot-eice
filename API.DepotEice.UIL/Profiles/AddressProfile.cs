using API.DepotEice.DAL.Entities;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using AutoMapper;

namespace API.DepotEice.UIL.Profiles
{
    public class AddressProfile : Profile
    {
        public AddressProfile()
        {
            CreateMap<AddressEntity, AddressModel>();

            CreateMap<AddressModel, AddressEntity>();

            CreateMap<AddressForm, AddressEntity>();
        }
    }
}
