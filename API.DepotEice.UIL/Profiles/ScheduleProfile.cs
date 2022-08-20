using API.DepotEice.BLL.Models;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using AutoMapper;

namespace API.DepotEice.UIL.Profiles
{
    public class ScheduleProfile : Profile
    {
        public ScheduleProfile()
        {
            CreateMap<ScheduleModel, ScheduleData>();
            CreateMap<ScheduleData, ScheduleModel>();
            CreateMap<ScheduleForm, ScheduleData>();
        }
    }
}
