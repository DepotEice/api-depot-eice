using API.DepotEice.DAL.Entities;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using AutoMapper;

namespace API.DepotEice.UIL.Profiles
{
    public class OpeningHoursProfile : Profile
    {
        public OpeningHoursProfile()
        {
            CreateMap<OpeningHoursEntity, OpeningHoursModel>();

            CreateMap<OpeningHoursForm, OpeningHoursEntity>();
        }
    }
}
