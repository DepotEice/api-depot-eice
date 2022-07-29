using API.DepotEice.BLL.Models;
using API.DepotEice.DAL.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL.Profiles
{
    public class OpeningHoursProfile : Profile
    {
        public OpeningHoursProfile()
        {
            // OpeningHoursModel --> OpeningHoursEntity

            CreateMap<OpeningHoursModel, OpeningHoursEntity>();

            // OpeningHoursEntity --> OpeningHoursModel

            CreateMap<OpeningHoursEntity, OpeningHoursModel>();
        }
    }
}
