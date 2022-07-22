using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.DAL.Mappers;
using API.DepotEice.Helpers.Exceptions;
using DevHopTools.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.DAL.Repositories
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly Connection _connection;

        public ScheduleRepository(Connection connection)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            _connection = connection;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DatabaseScalarNullException"></exception>
        public int Create(ScheduleEntity entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            Command command = new Command("spCreateSchedule", true);

            command.AddParameter("title", entity.Title);
            command.AddParameter("details", entity.Details);
            command.AddParameter("startsAt", entity.StartsAt);
            command.AddParameter("endsAt", entity.EndsAt);
            command.AddParameter("moduleId", entity.ModuleId);

            string? scalarResult = _connection.ExecuteScalar(command).ToString();

            if (string.IsNullOrEmpty(scalarResult))
            {
                throw new DatabaseScalarNullException(nameof(scalarResult));
            }

            return int.Parse(scalarResult);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"></exception>
        public bool Delete(ScheduleEntity entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            Command command = new Command("spDeleteSchedule", true);

            command.AddParameter("id", entity.Id);

            return _connection.ExecuteNonQuery(command) > 0;
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IEnumerable<ScheduleEntity> GetAll()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public ScheduleEntity? GetByKey(int key)
        {
            if (key <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(key));
            }

            Command command = new Command("spGetSchedule", true);

            command.AddParameter("id", key);

            return _connection
                .ExecuteReader(command, schedule => Mapper.DbToSchedule(schedule))
                .SingleOrDefault();
        }

        /// <summary>
        /// Retrieve every <see cref="ScheduleEntity"/> records from the database linked to a
        /// module <see cref="ModuleEntity"/> by its ID
        /// </summary>
        /// <param name="moduleId">
        /// The linked <see cref="ModuleEntity"/>'s ID
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="ScheduleEntity"/>. If no data is found,
        /// returns an empty <see cref="IEnumerable{T}"/>
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public IEnumerable<ScheduleEntity> GetModuleSchedules(int moduleId)
        {
            if (moduleId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(moduleId));
            }

            Command command = new Command("spGetModuleSchedules", true);

            command.AddParameter("moduleId", moduleId);

            return _connection
                .ExecuteReader(command, schedule => Mapper.DbToSchedule(schedule));
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"></exception>
        public bool Update(ScheduleEntity entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            Command command = new Command("spUpdateSchedule", true);

            command.AddParameter("id", entity.Id);
            command.AddParameter("title", entity.Title);
            command.AddParameter("details", entity.Details);

            return _connection.ExecuteNonQuery(command) > 0;
        }
    }
}
