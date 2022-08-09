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
    }
}
