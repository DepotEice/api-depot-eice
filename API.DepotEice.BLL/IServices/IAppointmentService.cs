using API.DepotEice.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL.IServices
{
    public interface IAppointmentService
    {
        IEnumerable<AppointmentModel> GetAppointments();
        AppointmentModel? GetAppointment(int id);
        AppointmentModel? CreateAppointment(AppointmentModel model);
        bool DeleteAppointment(int id);
        bool AcceptAppointment(int id);
    }
}
