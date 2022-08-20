using API.DepotEice.BLL.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL.IServices
{
    public interface IAppointmentService
    {
        IEnumerable<AppointmentDto> GetAppointments();
        AppointmentDto? GetAppointment(int id);
        AppointmentDto? CreateAppointment(AppointmentDto model);
        bool DeleteAppointment(int id);
        bool AcceptAppointment(int id);
    }
}
