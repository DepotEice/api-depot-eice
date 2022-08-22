using API.DepotEice.BLL.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL.IServices
{
    public interface IOpeningHoursService
    {
        OpeningHoursDto? CreateOpeningHours(OpeningHoursDto model);
        OpeningHoursDto? UpdateOpeningHours(OpeningHoursDto model);
        IEnumerable<OpeningHoursDto> GetOpeningHours();
        bool DeleteOpeningHours(int id);
    }
}
