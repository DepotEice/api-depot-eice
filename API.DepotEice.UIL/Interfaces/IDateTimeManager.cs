using API.DepotEice.UIL.Models.Forms;

namespace API.DepotEice.UIL.Interfaces
{
    public interface IDateTimeManager
    {
        bool DateTimeIsAvailable(AppointmentForm appointment);
        bool OpeningHoursAvailable(OpeningHoursForm openingHours, int id = 0);
    }
}
