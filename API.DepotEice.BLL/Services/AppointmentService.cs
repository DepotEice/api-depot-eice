using API.DepotEice.BLL.Extensions;
using API.DepotEice.BLL.IServices;
using API.DepotEice.BLL.Models;
using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.Helpers.Exceptions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IUserRepository _userRepository;

        public AppointmentService(ILogger<AppointmentService> logger, IMapper mapper,
            IAppointmentRepository appointmentRepository, IUserRepository userRepository)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (mapper is null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            if (appointmentRepository is null)
            {
                throw new ArgumentNullException(nameof(appointmentRepository));
            }

            if (userRepository is null)
            {
                throw new ArgumentNullException(nameof(userRepository));
            }

            _logger = logger;
            _mapper = mapper;
            _appointmentRepository = appointmentRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Accepte an appointment
        /// </summary>
        /// <param name="id">
        /// <see cref="AppointmentModel"/> ID
        /// </param>
        /// <returns>
        /// <c>true</c> If it was succesfully modified. <c>false</c> Otherwise
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public bool AcceptAppointment(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException($"Appointment's ID must be greater " +
                    $"than 0!");
            }

            return _appointmentRepository.AcceptAppointment(id);
        }

        /// <summary>
        /// Create an appointment
        /// </summary>
        /// <param name="model">
        /// Instance of <see cref="AppointmentModel"/>
        /// </param>
        /// <returns>
        /// The newly created <see cref="AppointmentModel"/>
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="MappedNullValueException"></exception>
        public AppointmentModel? CreateAppointment(AppointmentModel model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            AppointmentEntity? appointmentToCreate = _mapper.Map<AppointmentEntity>(model);

            if (appointmentToCreate is null)
            {
                throw new MappedNullValueException($"Mapping returned null value");
            }

            int newId = _appointmentRepository.Create(appointmentToCreate);

            AppointmentEntity? appointmentFromRepo = _appointmentRepository.GetByKey(newId);

            if (appointmentFromRepo is null)
            {
                _logger.LogError("{date} - Could not retrieve appointment with ID : \"{id}\"!",
                    DateTime.Now, newId);

                return null;
            }

            UserEntity? appointmentUser = _userRepository.GetByKey(appointmentFromRepo.UserId);

            if (appointmentUser is null)
            {
                _logger.LogError("{date} - The linked user with ID : \"{userId}\" to the " +
                    "appointment with ID : \"{appointmentId}\" does not exist.",
                    DateTime.Now, appointmentFromRepo.UserId, appointmentFromRepo.Id);

                return null;
            }

            AppointmentModel appointment = _mapper.MergeInto<AppointmentModel>(
                appointmentFromRepo, _mapper.Map<UserModel>(appointmentUser));

            return appointment;
        }

        /// <summary>
        /// Delete an appointment
        /// </summary>
        /// <param name="id">
        /// The ID of the appointment to delete
        /// </param>
        /// <returns>
        /// <c>true</c> If the appointment was successfully deleted. <c>false</c> If no appointment
        /// with <paramref name="id"/> exists in the database or if removal failed in the 
        /// repository
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public bool DeleteAppointment(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(id)} must be greater " +
                    $"than 0");
            }

            AppointmentEntity? appointmentFromRepo = _appointmentRepository.GetByKey(id);

            if (appointmentFromRepo is null)
            {
                _logger.LogWarning("{date} - There is no appointment in the database with " +
                    "ID : \"{id}\"", DateTime.Now, id);
                return false;
            }

            return _appointmentRepository.Delete(appointmentFromRepo);
        }

        /// <summary>
        /// Retrieve an appointment from the repository
        /// </summary>
        /// <param name="id">
        /// The ID of the appointment
        /// </param>
        /// <returns>
        /// <c>null</c> If no appointment with the <paramref name="id"/> exist. An Instance of
        /// <see cref="AppointmentModel"/> otherwise.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public AppointmentModel? GetAppointment(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(id)} must be greater " +
                    $"than 0!");
            }

            AppointmentEntity? appointmentFromRepo = _appointmentRepository.GetByKey(id);

            if (appointmentFromRepo is null)
            {
                _logger.LogError("{date} - Could not retrieve any appointment from the " +
                    "database with ID :\"{id}\"!", DateTime.Now, id);

                return null;
            }

            UserEntity? appointmentUser = _userRepository.GetByKey(appointmentFromRepo.UserId);

            if (appointmentUser is null)
            {
                _logger.LogError("{date} - Could not retrieve User with ID \"{userId}\" " +
                    "linked to the appointment with ID \"{appointmentId}\"",
                    DateTime.Now, appointmentFromRepo.UserId, appointmentFromRepo.Id);

                return null;
            }

            AppointmentModel appointment = _mapper.MergeInto<AppointmentModel>(
                 appointmentFromRepo, _mapper.Map<UserModel>(appointmentUser));

            return appointment;
        }

        /// <summary>
        /// Retrieve a list of appointments from the repository
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="AppointmentModel"/>.
        /// </returns>
        public IEnumerable<AppointmentModel> GetAppointments()
        {
            IEnumerable<AppointmentEntity> appointmentEntities = _appointmentRepository.GetAll();

            foreach (AppointmentEntity appointmentEntity in appointmentEntities)
            {
                UserEntity? appointmentUser = _userRepository.GetByKey(appointmentEntity.UserId);

                if (appointmentUser is null)
                {
                    _logger.LogError("{date} - Appointment with ID : \"{appointmentId}\" " +
                        "could not retrieve linked user with ID \"{userId}\"",
                        DateTime.Now, appointmentEntity.Id, appointmentEntity.UserId);
                }
                else
                {
                    AppointmentModel appointment = _mapper.MergeInto<AppointmentModel>(
                        appointmentEntity, _mapper.Map<UserModel>(appointmentUser));

                    yield return appointment;
                }
            }
        }
    }
}
