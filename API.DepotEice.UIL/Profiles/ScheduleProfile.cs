using API.DepotEice.DAL.Entities;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using AutoMapper;

namespace API.DepotEice.UIL.Profiles
{
    public class ScheduleProfile : Profile
    {
        public ScheduleProfile()
        {
            CreateMap<ScheduleEntity, ScheduleModel>();

            CreateMap<ScheduleForm, ScheduleEntity>();
        }
    }
}
