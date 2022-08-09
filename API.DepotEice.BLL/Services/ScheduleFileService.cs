using API.DepotEice.BLL.IServices;
using API.DepotEice.BLL.Models;
using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL.Services
{
    public class ScheduleFileService : IScheduleFileService
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IScheduleFileRepository _scheduleFileRepository;
        private readonly IScheduleRepository _scheduleRepository;

        public ScheduleFileService(ILogger<ScheduleFileService> logger, IMapper mapper,
            IScheduleFileRepository scheduleFileRepository, IScheduleRepository scheduleRepository)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (mapper is null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            if (scheduleFileRepository is null)
            {
                throw new ArgumentNullException(nameof(scheduleFileRepository));
            }

            if (scheduleRepository is null)
            {
                throw new ArgumentNullException(nameof(scheduleRepository));
            }

            _logger = logger;
            _mapper = mapper;
            _scheduleFileRepository = scheduleFileRepository;
            _scheduleRepository = scheduleRepository;
        }

        /// <summary>
        /// Create a ScheduleFile in the database
        /// </summary>
        /// <param name="model"></param>
        /// <returns>
        /// <c>null</c> If the ScheduleFile could not be created or if the ScheduleFile retrieved 
        /// from the database with the newly created ID is null or if the associated Schedule is 
        /// null. Otherwise, an instance of <see cref="ScheduleFileData"/>
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public ScheduleFileData? CreateScheduleFile(ScheduleFileData model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            ScheduleFileEntity scheduleFileToCreate = _mapper.Map<ScheduleFileEntity>(model);

            int newId = _scheduleFileRepository.Create(scheduleFileToCreate);

            if (newId <= 0)
            {
                _logger.LogWarning(
                    "{date} - The ScheduleFile could not be created!",
                    DateTime.Now);

                return null;
            }

            ScheduleFileEntity? scheduleFileFromRepo = _scheduleFileRepository.GetByKey(newId);

            if (scheduleFileFromRepo is null)
            {
                _logger.LogError(
                    "{date} - Retrieved ScheduleFile with the newly received ID \"{id}\" " +
                    "does not exist in the database!",
                    DateTime.Now, newId);

                return null;
            }

            ScheduleEntity? scheduleFromRepo =
                _scheduleRepository.GetByKey(scheduleFileFromRepo.ScheduleId);

            if (scheduleFromRepo is null)
            {
                _logger.LogError(
                    "{date} - The Schedule with ID \"{scheduleId}\" related to the " +
                    "ScheduleFile with ID \"{scheduleFileId}\" does not exist in the database!",
                    DateTime.Now, scheduleFileFromRepo.ScheduleId, scheduleFileFromRepo.Id);

                return null;
            }

            ScheduleFileData scheduleFileModel =
                _mapper.Map<ScheduleFileData>(scheduleFileFromRepo);

            ScheduleData scheduleModel = _mapper.Map<ScheduleData>(scheduleFromRepo);

            scheduleFileModel.Schedule = scheduleModel;

            return scheduleFileModel;
        }

        /// <summary>
        /// Delete a ScheduleFile from the database
        /// </summary>
        /// <param name="id">
        /// The ID of the ScheduleFile to delete
        /// </param>
        /// <returns>
        /// <c>true</c> If the ScheduleFile was successfully deleted. <c>false</c> If the 
        /// ScheduleFile does not exist or if the operation failed
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public bool DeleteScheduleFile(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            ScheduleFileEntity? scheduleFileFromRepo = _scheduleFileRepository.GetByKey(id);

            if (scheduleFileFromRepo is null)
            {
                _logger.LogWarning(
                    "{date} - There is no ScheduleFile in the database with ID \"{id}\"",
                    DateTime.Now, id);

                return false;
            }

            return _scheduleFileRepository.Delete(scheduleFileFromRepo);
        }

        /// <summary>
        /// Retrieve a ScheduleFile from the database
        /// </summary>
        /// <param name="id">
        /// The ID of the ScheduleFile to retrieve
        /// </param>
        /// <returns>
        /// <c>null</c> If the ScheduleFile does not exist in the database or if the associated 
        /// Schedule does not exist. Otherwise, an instance of <see cref="ScheduleFileData"/>
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public ScheduleFileData? GetScheduleFile(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            ScheduleFileEntity? scheduleFileFromRepo = _scheduleFileRepository.GetByKey(id);

            if (scheduleFileFromRepo is null)
            {
                _logger.LogWarning(
                    "{date} - There is no ScheduleFile with ID \"{id}\" in the database!",
                    DateTime.Now, id);

                return null;
            }

            ScheduleEntity? scheduleFromRepo =
                 _scheduleRepository.GetByKey(scheduleFileFromRepo.ScheduleId);

            if (scheduleFromRepo is null)
            {
                _logger.LogError(
                    "{date} - The Schedule with ID \"{scheduleId}\" related to the " +
                    "ScheduleFile with ID \"{scheduleFileId}\" does not exist in the database!",
                    DateTime.Now, scheduleFileFromRepo.ScheduleId, scheduleFileFromRepo.Id);

                return null;
            }

            ScheduleFileData scheduleFileModel =
                _mapper.Map<ScheduleFileData>(scheduleFileFromRepo);

            ScheduleData scheduleModel = _mapper.Map<ScheduleData>(scheduleFromRepo);

            scheduleFileModel.Schedule = scheduleModel;

            return scheduleFileModel;
        }

        /// <summary>
        /// Retrieve all ScheduleFiles related to a Schedule
        /// </summary>
        /// <param name="scheduleId">
        /// The ID of the Schedule
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="ScheduleData"/>
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public IEnumerable<ScheduleFileData> GetScheduleFiles(int scheduleId)
        {
            if (scheduleId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(scheduleId));
            }

            IEnumerable<ScheduleFileEntity> scheduleFilesFromRepo =
                _scheduleFileRepository.GetScheduleFiles(scheduleId);

            foreach (ScheduleFileEntity scheduleFileFromRepo in scheduleFilesFromRepo)
            {
                ScheduleEntity? scheduleFromRepo =
                    _scheduleRepository.GetByKey(scheduleFileFromRepo.ScheduleId);

                if (scheduleFromRepo is null)
                {
                    _logger.LogWarning(
                        "{date} - The Schedule with ID \"{scheduleId}\" related to the " +
                        "ScheduleFile with ID \"{scheduleFileId}\" does not exist in the database !",
                        DateTime.Now, scheduleFileFromRepo.ScheduleId, scheduleFileFromRepo.Id);
                }
                else
                {
                    ScheduleFileData scheduleFileModel =
                        _mapper.Map<ScheduleFileData>(scheduleFileFromRepo);

                    scheduleFileModel.Schedule = _mapper.Map<ScheduleData>(scheduleFromRepo);

                    yield return scheduleFileModel;
                }
            }
        }
    }
}
