﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.DAL.Entities
{
    /// <summary>
    /// Represent the <c>ScheduleFiles</c> table in the database
    /// </summary>
    public class ScheduleFileEntity
    {
        /// <summary>
        /// Represent the <c>Id</c> column in the database
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Represent the <c>FilePath</c> column in the database
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// Represent the <c>ScheduleId</c> column in the database
        /// </summary>
        public int ScheduleId { get; set; }
    }
}
