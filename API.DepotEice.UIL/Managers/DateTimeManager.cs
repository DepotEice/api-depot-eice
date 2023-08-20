using API.DepotEice.DAL.IRepositories;
using API.DepotEice.UIL.Interfaces;
using API.DepotEice.UIL.Models.Forms;
using DevHopTools.Mappers;

namespace API.DepotEice.UIL.Managers
{
    /// <summary>
    /// Date time manager class
    /// </summary>
    public class DateTimeManager : IDateTimeManager
    {
        private readonly ILogger _logger;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IOpeningHoursRepository _openingHoursRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="appointmentRepository"></param>
        /// <param name="openingHoursRepository"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public DateTimeManager(ILogger<DateTimeManager> logger, IAppointmentRepository appointmentRepository,
            IOpeningHoursRepository openingHoursRepository)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (appointmentRepository is null)
            {
                throw new ArgumentNullException(nameof(appointmentRepository));
            }

            if (openingHoursRepository is null)
            {
                throw new ArgumentNullException(nameof(openingHoursRepository));
            }

            _logger = logger;
            _appointmentRepository = appointmentRepository;
            _openingHoursRepository = openingHoursRepository;
        }

        /// <summary>
        /// Verify if the date and time is available for an appointment
        /// </summary>
        /// <param name="appointment"></param>
        /// <returns>
        /// <c>true</c> If the datetime is available, <c>false</c> otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool DateTimeIsAvailable(AppointmentForm appointment)
        {
            if (appointment is null)
            {
                throw new ArgumentNullException(nameof(appointment));
            }

            var startDate = appointment.StartAt;
            var endDate = appointment.EndAt;

            if (startDate < DateTime.Now || endDate < DateTime.Now)
            {
                return false;
            }

            var openingHoursFromRepo = _openingHoursRepository.GetAll();

            if (!openingHoursFromRepo.Any(oh => oh.OpenAt <= startDate && oh.CloseAt >= endDate))
            {
                return false;
            }

            var appointmentFromRepo = _appointmentRepository.GetAll();

            bool appointmentExist = appointmentFromRepo.Any(a => a.StartAt == startDate && a.EndAt == endDate);

            if (appointmentExist)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Verify if the opening hours is available
        /// </summary>
        /// <param name="openingHours">the opening hours form</param>
        /// <param name="id">the id of the opening hours in the database</param>
        /// <returns>
        /// <c>true</c> If the opening hours is available, <c>false</c> otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool OpeningHoursAvailable(OpeningHoursForm openingHours, int id = 0)
        {
            if (openingHours is null)
            {
                throw new ArgumentNullException(nameof(openingHours));
            }

            return !_openingHoursRepository.GetAll().Any(oh =>
                openingHours.OpenAt >= oh.OpenAt &&
                openingHours.CloseAt <= oh.CloseAt &&
                oh.Id != id);
        }
    }
}
