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
    }
}
