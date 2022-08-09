using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using AutoMapper;

namespace API.DepotEice.UIL.Profiles
{
    public class ScheduleProfile : Profile
    {
        public ScheduleProfile()
        {
            CreateMap<ScheduleModel, BLL.Models.ScheduleData>();
            CreateMap<BLL.Models.ScheduleData, ScheduleModel>();
            CreateMap<ScheduleForm, BLL.Models.ScheduleData>();
        }
    }
}
