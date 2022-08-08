using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using AutoMapper;

namespace API.DepotEice.UIL.Profiles
{
    public class ScheduleProfile : Profile
    {
        public ScheduleProfile()
        {
            CreateMap<ScheduleModel, BLL.Models.ScheduleModel>();
            CreateMap<ScheduleForm, BLL.Models.ScheduleModel>();
        }
    }
}
