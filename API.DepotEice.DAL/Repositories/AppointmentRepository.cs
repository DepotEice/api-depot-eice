using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.DAL.Mappers;
using API.DepotEice.Helpers.Exceptions;
using DevHopTools.Connection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.DAL.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly Connection _connection;

        public AppointmentRepository(Connection connection)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            _connection = connection;
        }

        // TODO : Add possibility to removed accepted
        /// <summary>
        /// Set the <c>Accepted</c> column's value to <c>true</c> for <c>Appointments</c> table in
        /// the database for the record selected by its ID given in parameter.
        /// </summary>
        /// <param name="appointmentId">
        /// The ID of the record to update. Throws an <see cref="ArgumentOutOfRangeException"/> if
        /// <paramref name="appointmentId"/> is <c>smaller</c> or <c>equals</c> to <c>0</c>
        /// </param>
        /// <returns>
        /// <c>true</c> If at least one row was affected in the database. <c>false</c> Otherwise
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public bool AcceptAppointment(int appointmentId)
        {
            if (appointmentId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(appointmentId));
            }

            Command command = new Command("spAcceptAppointment", true);
            command.AddParameter("id", appointmentId);

            return _connection.ExecuteNonQuery(command) > 0;
        }

        public int Create(AppointmentEntity entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            Command command = new Command("spCreateAppointment", true);

            command.AddParameter("startAt", entity.StartAt);
            command.AddParameter("endAt", entity.EndAt);
            command.AddParameter("userId", entity.UserId);

            string? scalarResult = _connection.ExecuteScalar(command).ToString();

            if (string.IsNullOrEmpty(scalarResult))
            {
                throw new DatabaseScalarNullException(nameof(scalarResult));
            }

            return int.Parse(scalarResult);
        }

        /// <summary>
        /// Delete the given <paramref name="entity"/> from the database if it exists
        /// </summary>
        /// <param name="entity">
        /// The entity from the database to delete. Throw a <see cref="ArgumentException"/> if it
        /// is <c>null</c>
        /// </param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool Delete(AppointmentEntity entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            Command command = new Command("spDeleteAppointment", true);

            command.AddParameter("id", entity.Id);

            return _connection.ExecuteNonQuery(command) > 0;
        }

        /// <summary>
        /// Retrieve all <see cref="AppointmentEntity"/> from the database
        /// </summary>
        /// <returns>
        /// A <see cref="IEnumerable{T}"/> of <see cref="AppointmentEntity"/>
        /// </returns>
        public IEnumerable<AppointmentEntity> GetAll()
        {
            Command command = new Command("SELECT * FROM [dbo].[Appointments]");

            return _connection.ExecuteReader(command,
                appointment => Mapper.DbToAppointmentEntity(appointment));
        }

        /// <summary>
        /// Return a unique <see cref="AppointmentEntity"/> from the database based on its ID 
        /// determined by <paramref name="key"/>.
        /// </summary>
        /// <param name="key">
        /// The desired record's ID. Throw a <see cref="ArgumentOutOfRangeException"/> if the 
        /// parameter is <c>equals</c> or <c>smaller</c> than <c>0</c>
        /// </param>
        /// <returns>
        /// <c>null</c> If there is no data in the database with the given ID or if there is more
        /// than one element returned. Otherwise return an object <see cref="AppointmentEntity"/>
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public AppointmentEntity? GetByKey(int key)
        {
            if (key <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(key));
            }

            Command command = new Command("spGetAppointment", true);

            command.AddParameter("id", key);

            return _connection
                .ExecuteReader(command, appointment => Mapper.DbToAppointmentEntity(appointment))
                .SingleOrDefault();
        }

        /// <summary>
        /// This method is inherited by the interface and is useless for this entity. This method
        /// will throw a <see cref="NotImplementedException"/>
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool Update(AppointmentEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
