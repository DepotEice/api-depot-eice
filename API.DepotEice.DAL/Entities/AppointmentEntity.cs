using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.DAL.Entities
{
    /// <summary>
    /// Entity describing the <c>Appointments</c> table in the database
    /// </summary>
    public class AppointmentEntity
    {
        /// <summary>
        /// Appointment ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Appointment start date and time (hh:mm)
        /// </summary>
        public DateTime StartAt { get; set; }

        /// <summary>
        /// Appointment end date and time (hh:mm)
        /// </summary>
        public DateTime EndAt { get; set; }

        /// <summary>
        /// Appointment acceptance status <c>true = Accepted</c> <c>false = Not accepted</c>
        /// </summary>
        public bool Accepted { get; set; }

        /// <summary>
        /// Appointment linked user's ID
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Instanciate an object <see cref="AppointmentEntity"/> and initialize all properties
        /// </summary>
        /// <param name="id">Appointment ID</param>
        /// <param name="startsAt">Date and time at which starts the appointment</param>
        /// <param name="endsAt">Date and time at which ends the appointment</param>
        /// <param name="accepted">Appointment acceptance flag</param>
        /// <param name="userId">Linked user's ID</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="DateTimeOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public AppointmentEntity(int id, DateTime startsAt, DateTime endsAt, bool accepted,
            string userId)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            if (startsAt >= endsAt)
            {
                throw new ArgumentOutOfRangeException(nameof(startsAt));
            }

            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            Id = id;
            StartAt = startsAt;
            EndAt = endsAt;
            Accepted = accepted;
            UserId = userId;
        }

        /// <summary>
        /// Instanciate an object <see cref="AppointmentEntity"/> for a creation in the database.
        /// The constructor is useful for the Stored Procedure <c>spCreateAppointment</c>
        /// </summary>
        /// <param name="startsAt">Date and time at which starts the appointment</param>
        /// <param name="endsAt">Date and time at which ends the appointment</param>
        /// <param name="accepted">Appointment acceptance flag</param>
        /// <param name="userId">Linked user's ID</param>
        /// <exception cref="DateTimeOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public AppointmentEntity(DateTime startsAt, DateTime endsAt, string userId)
        {
            if (startsAt >= endsAt)
            {
                throw new ArgumentOutOfRangeException(nameof(startsAt));
            }

            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            Id = 0;
            StartAt = startsAt;
            EndAt = endsAt;
            UserId = userId;
        }
    }
}
