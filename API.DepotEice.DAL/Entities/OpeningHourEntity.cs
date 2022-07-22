using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.DAL.Entities
{
    /// <summary>
    /// Represent the <c>OpeningHours</c> table in the database
    /// </summary>
    public class OpeningHoursEntity
    {
        /// <summary>
        /// Represent the <c>Id</c> column in the database
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Represent the <c>OpenAt</c> column in the database
        /// </summary>
        public DateTime OpenAt { get; set; }

        /// <summary>
        /// Represent the <c>CloseAt</c> column in the database
        /// </summary>
        public DateTime CloseAt { get; set; }

        /// <summary>
        /// Instanciate an object <see cref="OpeningHoursEntity"/> with all its properties
        /// </summary>
        /// <param name="id">Opening hour's id</param>
        /// <param name="openAt">Opening date and time</param>
        /// <param name="closeAt">Closing date and time</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="DateTimeOutOfRangeException"></exception>
        public OpeningHoursEntity(int id, DateTime openAt, DateTime closeAt)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            if (openAt >= closeAt)
            {
                throw new ArgumentOutOfRangeException(nameof(openAt));
            }

            if (closeAt <= openAt)
            {
                throw new ArgumentOutOfRangeException(nameof(closeAt));
            }

            Id = id;
            OpenAt = openAt;
            CloseAt = closeAt;
        }

        /// <summary>
        /// Instanciate an object <see cref="OpeningHoursEntity"/> with all its properties except
        /// <see cref="Id"/> that is set to 0
        /// </summary>
        /// <param name="openAt">Opening date and time</param>
        /// <param name="closeAt">Closing date and time</param>
        /// <exception cref="DateTimeOutOfRangeException"></exception>
        public OpeningHoursEntity(DateTime openAt, DateTime closeAt)
        {
            if (openAt >= closeAt)
            {
                throw new ArgumentOutOfRangeException(nameof(openAt));
            }

            if (closeAt <= openAt)
            {
                throw new ArgumentOutOfRangeException(nameof(closeAt));
            }

            Id = 0;
            OpenAt = openAt;
            CloseAt = closeAt;
        }
    }
}
