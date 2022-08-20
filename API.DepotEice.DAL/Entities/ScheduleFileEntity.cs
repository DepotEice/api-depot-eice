using System;
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
        public string FilePath { get; set; }

        /// <summary>
        /// Represent the <c>ScheduleId</c> column in the database
        /// </summary>
        public int ScheduleId { get; set; }

        ///// <summary>
        ///// Instanciate an object <see cref="ScheduleFileEntity"/> with all its properties
        ///// </summary>
        ///// <param name="id">ScheduleFiles Id</param>
        ///// <param name="filePath">The complete file path</param>
        ///// <param name="scheduleId">The related <see cref="ScheduleEntity"/>'s ID</param>
        ///// <exception cref="ArgumentOutOfRangeException"></exception>
        ///// <exception cref="ArgumentNullException"></exception>
        //public ScheduleFileEntity(int id, string filePath, int scheduleId)
        //{
        //    if (id < 0)
        //    {
        //        throw new ArgumentOutOfRangeException(nameof(id));
        //    }

        //    if (string.IsNullOrEmpty(FilePath))
        //    {
        //        throw new ArgumentNullException(nameof(filePath));
        //    }

        //    if (scheduleId <= 0)
        //    {
        //        throw new ArgumentOutOfRangeException(nameof(scheduleId));
        //    }

        //    Id = id;
        //    FilePath = filePath;
        //    ScheduleId = scheduleId;
        //}

        ///// <summary>
        ///// Instanciate an object <see cref="ScheduleFileEntity"/> with all its properties except
        ///// <see cref="Id"/> which value is 0
        ///// </summary>
        ///// <param name="filePath">The complete file path</param>
        ///// <param name="scheduleId">The related <see cref="ScheduleEntity"/>'s 
        ///// <see cref="ScheduleEntity.Id"/></param>
        ///// <exception cref="ArgumentNullException"></exception>
        ///// <exception cref="ArgumentOutOfRangeException"></exception>
        //public ScheduleFileEntity(string filePath, int scheduleId)
        //{
        //    if (string.IsNullOrEmpty(filePath))
        //    {
        //        throw new ArgumentNullException(nameof(filePath));
        //    }

        //    if (scheduleId <= 0)
        //    {
        //        throw new ArgumentOutOfRangeException(nameof(scheduleId));
        //    }

        //    Id = 0;
        //    FilePath = filePath;
        //    ScheduleId = scheduleId;
        //}
    }
}
