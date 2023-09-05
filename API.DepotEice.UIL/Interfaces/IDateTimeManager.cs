using API.DepotEice.UIL.Models.Forms;

namespace API.DepotEice.UIL.Interfaces
{
    /// <summary>
    /// Date time manager interface
    /// </summary>
    public interface IDateTimeManager
    {
        /// <summary>
        /// Verify if the date and time is available for an appointment
        /// </summary>
        /// <param name="appointment"></param>
        /// <returns>
        /// <c>true</c> If the datetime is available, <c>false</c> otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        bool DateTimeIsAvailable(AppointmentForm appointment);

        /// <summary>
        /// Verify if the opening hours is available
        /// </summary>
        /// <param name="openingHours">the opening hours form</param>
        /// <param name="id">the id of the opening hours in the database</param>
        /// <returns>
        /// <c>true</c> If the opening hours is available, <c>false</c> otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        bool OpeningHoursAvailable(OpeningHoursForm openingHours, int id = 0);
    }
}
