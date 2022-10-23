using API.DepotEice.DAL.IRepositories;
using API.DepotEice.UIL.Interfaces;
using API.DepotEice.UIL.Models.Forms;
using DevHopTools.Mappers;

namespace API.DepotEice.UIL.Managers
{
    public class DateTimeManager : IDateTimeManager
    {
        private readonly ILogger _logger;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IOpeningHoursRepository _openingHoursRepository;

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

        public bool DateTimeIsAvailable(AppointmentForm appointment)
        {
            if (appointment is null)
            {
                throw new ArgumentNullException(nameof(appointment));
            }

            var startDate = appointment.StartAt;
            var endDate = appointment.EndAt;

            if (!_openingHoursRepository.GetAll().Any(oh => oh.OpenAt < startDate && oh.CloseAt > endDate))
            {
                return false;
            }

            if (_appointmentRepository.GetAll().Any(oh => (startDate > oh.StartAt && startDate < oh.EndAt) ||
                (endDate > oh.StartAt && endDate < oh.EndAt)))
            {
                return false;
            }

            return true;
        }

        public bool OpeningHoursAvailable(OpeningHoursForm openingHours, int id = 0)
        {
            if (openingHours is null)
            {
                throw new ArgumentNullException(nameof(openingHours));
            }

            return !_openingHoursRepository.GetAll().Any(oh =>
                openingHours.OpenAt >= oh.OpenAt &&
                openingHours.CloseAt <= oh.CloseAt &&
                (id != 0 ? oh.Id != id : true));
        }
    }
}
