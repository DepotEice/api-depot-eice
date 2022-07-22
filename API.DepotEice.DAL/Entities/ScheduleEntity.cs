using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.DAL.Entities
{
    /// <summary>
    /// Represent the <c>Schedules</c> table in the database
    /// </summary>
    public class ScheduleEntity
    {
        /// <summary>
        /// Represent the <c>Id</c> column in the database
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Represent the <c>Title</c> column in the database
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Represent the <c>Details</c> column in the database
        /// </summary>
        public string? Details { get; set; }

        /// <summary>
        /// Represent the <c>StartsAt</c> column in the database
        /// </summary>
        public DateTime StartsAt { get; set; }

        /// <summary>
        /// Represent the <c>EndsAt</c> column in the database
        /// </summary>
        public DateTime EndsAt { get; set; }

        /// <summary>
        /// Represent the <c>ModuleId</c> column and foreign key in the database
        /// </summary>
        public int ModuleId { get; set; }

        /// <summary>
        /// Instanciate an object <see cref="ScheduleEntity"/> with all its properties
        /// </summary>
        /// <param name="id">Schedule's Id</param>
        /// <param name="title">Schedule's title</param>
        /// <param name="details">Schedule's details</param>
        /// <param name="startsAt">Schedule's start date and time</param>
        /// <param name="endsAt">Schedule's end date and time</param>
        /// <param name="moduleId"><see cref="ModuleEntity.Id"/> property of 
        /// <see cref="ModuleEntity"/></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DateTimeOutOfRangeException"></exception>
        public ScheduleEntity(int id, string? title, string? details, DateTime startsAt,
            DateTime endsAt, int moduleId)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentNullException(nameof(title));
            }

            if (string.IsNullOrEmpty(details))
            {
                throw new ArgumentNullException(nameof(details));
            }

            if (startsAt >= endsAt)
            {
                throw new ArgumentOutOfRangeException(nameof(startsAt));
            }

            if (endsAt <= startsAt)
            {
                throw new ArgumentOutOfRangeException(nameof(endsAt));
            }

            if (ModuleId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(moduleId));
            }

            Id = id;
            Title = title;
            Details = details;
            StartsAt = startsAt;
            EndsAt = endsAt;
            ModuleId = moduleId;
        }

        /// <summary>
        /// Instanciate an object <see cref="ScheduleEntity"/> with all its properties except
        /// <see cref="Id"/> which value is 0
        /// </summary>
        /// <param name="title">Schedule's title</param>
        /// <param name="details">Schedule's details</param>
        /// <param name="startsAt">Schedule's start date and time</param>
        /// <param name="endsAt">Schedule's end date and time</param>
        /// <param name="moduleId"><see cref="ModuleEntity.Id"/> property of
        /// <see cref="ModuleEntity"/></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DateTimeOutOfRangeException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public ScheduleEntity(string title, string details, DateTime startsAt, DateTime endsAt,
            int moduleId)
        {
            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentNullException(nameof(title));
            }

            if (string.IsNullOrEmpty(details))
            {
                throw new ArgumentNullException(nameof(details));
            }

            if (startsAt >= endsAt)
            {
                throw new ArgumentOutOfRangeException(nameof(startsAt));
            }

            if (endsAt <= startsAt)
            {
                throw new ArgumentOutOfRangeException(nameof(endsAt));
            }

            if (moduleId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(moduleId));
            }

            Id = 0;
            Title = title;
            Details = details;
            StartsAt = startsAt;
            EndsAt = endsAt;
            ModuleId = moduleId;
        }
    }
}
