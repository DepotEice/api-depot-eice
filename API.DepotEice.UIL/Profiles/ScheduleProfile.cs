using API.DepotEice.BLL.Dtos;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using AutoMapper;

namespace API.DepotEice.UIL.Profiles
{
    public class ScheduleProfile : Profile
    {
        public ScheduleProfile()
        {
            CreateMap<ScheduleModel, ScheduleDto>();
            CreateMap<ScheduleDto, ScheduleModel>();
            CreateMap<ScheduleForm, ScheduleDto>();
        }
    }
}
