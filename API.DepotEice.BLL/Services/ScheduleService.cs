﻿using API.DepotEice.BLL.IServices;
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
    public class ScheduleService : IScheduleService
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IModuleRepository _moduleRepository;
        private readonly IScheduleFileRepository _scheduleFileRepository;

        public ScheduleService(ILogger<ScheduleService> logger, IMapper mapper,
            IScheduleRepository scheduleRepository, IModuleRepository moduleRepository,
            IScheduleFileRepository scheduleFileRepository)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (mapper is null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            if (scheduleRepository is null)
            {
                throw new ArgumentNullException(nameof(scheduleRepository));
            }

            if (moduleRepository is null)
            {
                throw new ArgumentNullException(nameof(moduleRepository));
            }

            if (scheduleFileRepository is null)
            {
                throw new ArgumentNullException(nameof(scheduleFileRepository));
            }

            _logger = logger;
            _mapper = mapper;
            _scheduleRepository = scheduleRepository;
            _moduleRepository = moduleRepository;
            _scheduleFileRepository = scheduleFileRepository;
        }

        /// <summary>
        /// Create a new Schedule in the database
        /// </summary>
        /// <param name="model">
        /// The Schedule to create
        /// </param>
        /// <returns>
        /// <c>null</c> If the creation failed or if the related Module does not exist. Otherwise,
        /// an instance of <see cref="ScheduleModel"/>
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public ScheduleModel? CreateSchedule(ScheduleModel model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            ScheduleEntity scheduleToCreate = _mapper.Map<ScheduleEntity>(model);

            int newId = _scheduleRepository.Create(scheduleToCreate);

            if (newId <= 0)
            {
                _logger.LogWarning(
                    "{date} - The Schedule creation failed and returned the following ID " +
                    "\"{id}\"!",
                    DateTime.Now, newId);

                return null;
            }

            ScheduleEntity? scheduleFromRepo = _scheduleRepository.GetByKey(newId);

            if (scheduleFromRepo is null)
            {
                _logger.LogError(
                    "{date} - The Schedule retrieved with the newly created ID \"{id}\" is " +
                    "null!",
                    DateTime.Now, newId);

                return null;
            }

            ModuleEntity? moduleFromRepo = _moduleRepository.GetByKey(newId);

            if (moduleFromRepo is null)
            {
                _logger.LogError(
                    "{date} - The Module with ID \"{moduleId}\" retrieved from the database " +
                    "for Schedule with ID \"{scheduleId}\" is null!",
                    DateTime.Now, scheduleFromRepo.ModuleId, scheduleFromRepo.Id);

                return null;
            }

            IEnumerable<ScheduleFileEntity> scheduleFilesFromRepo =
                _scheduleFileRepository.GetScheduleFiles(newId);

            ScheduleModel scheduleModel = _mapper.Map<ScheduleModel>(scheduleFromRepo);

            scheduleModel.Module = _mapper.Map<ModuleModel>(moduleFromRepo);

            scheduleModel.ScheduleFiles =
                _mapper.Map<IEnumerable<ScheduleFileModel>>(scheduleFilesFromRepo);

            return scheduleModel;
        }

        /// <summary>
        /// Delete a Schedule from the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public bool DeleteSchedule(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            ScheduleEntity? scheduleFromRepo = _scheduleRepository.GetByKey(id);

            if (scheduleFromRepo is null)
            {
                _logger.LogWarning(
                    "{date} - There is no Schedule with ID \"{id}\" in the database!",
                    DateTime.Now, id);

                return false;
            }

            return _scheduleRepository.Delete(scheduleFromRepo);
        }

        /// <summary>
        /// Retrieve all Schedules related to a Module
        /// </summary>
        /// <param name="moduleId">
        /// The ID of the module
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="ScheduleModel"/>
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public IEnumerable<ScheduleModel> GetModuleSchedules(int moduleId)
        {
            if (moduleId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(moduleId));
            }

            IEnumerable<ScheduleEntity> schedulesFromRepo =
                _scheduleRepository.GetModuleSchedules(moduleId);

            ModuleEntity? moduleFromRepo = _moduleRepository.GetByKey(moduleId);

            if (moduleFromRepo is null)
            {
                _logger.LogError(
                    "{date} - The Module with ID \"{moduleId}\" does not exist!",
                    DateTime.Now, moduleId);

                yield break;
            }

            foreach (ScheduleEntity scheduleFromRepo in schedulesFromRepo)
            {
                ScheduleModel schedule = _mapper.Map<ScheduleModel>(scheduleFromRepo);

                IEnumerable<ScheduleFileEntity> scheduleFilesFromRepo =
                    _scheduleFileRepository.GetScheduleFiles(scheduleFromRepo.Id);

                schedule.ScheduleFiles =
                    _mapper.Map<IEnumerable<ScheduleFileModel>>(scheduleFilesFromRepo);

                schedule.Module = _mapper.Map<ModuleModel>(moduleFromRepo);

                yield return schedule;
            }
        }

        /// <summary>
        /// Retrieve a Schedule from the database
        /// </summary>
        /// <param name="id">
        /// The ID of the Schedule
        /// </param>
        /// <returns>
        /// <c>null</c> If the Schedule does not exist or if the related Module does not exist.
        /// Otherwise, an instance of <see cref="ScheduleModel"/>
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public ScheduleModel? GetSchedule(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            ScheduleEntity? scheduleFromRepo = _scheduleRepository.GetByKey(id);

            if (scheduleFromRepo is null)
            {
                _logger.LogWarning(
                    "{date} - There is no Schedule with ID \"{id}\" in the database!",
                    DateTime.Now, id);

                return null;
            }

            ModuleEntity? moduleFromRepo = _moduleRepository.GetByKey(scheduleFromRepo.ModuleId);

            if (moduleFromRepo is null)
            {
                _logger.LogError(
                    "{date} - The Module with ID \"{moduleId}\" related to the Schedule " +
                    "with ID \"{scheduleId}\" does not exist!",
                    DateTime.Now, scheduleFromRepo.ModuleId, scheduleFromRepo.Id);

                return null;
            }

            IEnumerable<ScheduleFileEntity> scheduleFilesFromRepo =
                _scheduleFileRepository.GetScheduleFiles(scheduleFromRepo.Id);

            ScheduleModel scheduleModel = _mapper.Map<ScheduleModel>(scheduleFromRepo);

            scheduleModel.Module = _mapper.Map<ModuleModel>(moduleFromRepo);

            scheduleModel.ScheduleFiles =
                _mapper.Map<IEnumerable<ScheduleFileModel>>(scheduleFilesFromRepo);

            return scheduleModel;
        }

        /// <summary>
        /// Update a Schedule in the database
        /// </summary>
        /// <param name="model">
        /// The Schedule to update
        /// </param>
        /// <returns>
        /// <c>null</c> If the Schedule does not exist or if the related module does not exist.
        /// Otherwise, an instance of updated <see cref="ScheduleModel"/>
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public ScheduleModel? UpdateSchedule(ScheduleModel model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            ScheduleEntity? scheduleToUpdate = _mapper.Map<ScheduleEntity>(model);

            if (!_scheduleRepository.Update(scheduleToUpdate))
            {
                _logger.LogWarning(
                    "{date} - Could not update the Schedule with ID \"{id}\"!",
                    DateTime.Now, model.Id);

                return null;
            }

            ScheduleEntity? scheduleFromRepo = _scheduleRepository.GetByKey(model.Id);

            if (scheduleFromRepo is null)
            {
                _logger.LogError(
                    "{date} - The updated Schedule with ID \"{id}\" returned null!",
                    DateTime.Now, model.Id);

                return null;
            }

            ModuleEntity? moduleFromRepo = _moduleRepository.GetByKey(scheduleFromRepo.ModuleId);

            if (moduleFromRepo is null)
            {
                _logger.LogError(
                    "{date} - The retrieval of the Module with ID \"{moduleID}\" related " +
                    "to the Schedule with ID \"{scheduleId}\" returned null!",
                    DateTime.Now, scheduleFromRepo.ModuleId, scheduleFromRepo.Id);

                return null;
            }

            IEnumerable<ScheduleFileEntity> scheduleFilesFromRepo =
                _scheduleFileRepository.GetScheduleFiles(scheduleFromRepo.Id);

            ScheduleModel scheduleModel = _mapper.Map<ScheduleModel>(scheduleFromRepo);

            scheduleModel.Module = _mapper.Map<ModuleModel>(moduleFromRepo);

            scheduleModel.ScheduleFiles =
                _mapper.Map<IEnumerable<ScheduleFileModel>>(scheduleFilesFromRepo);

            return scheduleModel;
        }
    }
}