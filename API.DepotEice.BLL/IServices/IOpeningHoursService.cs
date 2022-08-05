using API.DepotEice.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL.IServices
{
    public interface IOpeningHoursService
    {
        OpeningHoursModel? CreateOpeningHours(OpeningHoursModel model);
        OpeningHoursModel? UpdateOpeningHours(OpeningHoursModel model);
        IEnumerable<OpeningHoursModel> GetOpeningHours();
        bool DeleteOpeningHours(int id);
    }
}
