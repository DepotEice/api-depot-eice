using API.DepotEice.BLL.IServices;
using API.DepotEice.BLL.Models;
using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using AutoMapper;
using Microsoft.Extensions.Logging;

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
        /// an instance of <see cref="ScheduleData"/>
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public ScheduleData? CreateSchedule(int moduleId, ScheduleData model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            //ScheduleEntity scheduleToCreate = _mapper.Map<ScheduleEntity>(model);
            ScheduleEntity scheduleToCreate = new ScheduleEntity()
            {
                Title = model.Title,
                Details = model.Details,
                StartsAt = model.StartsAt,
                EndsAt = model.EndsAt,
                ModuleId = moduleId
            };

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

            ScheduleData scheduleModel = _mapper.Map<ScheduleData>(scheduleFromRepo);

            scheduleModel.Module = _mapper.Map<ModuleData>(moduleFromRepo);

            scheduleModel.ScheduleFiles =
                _mapper.Map<IEnumerable<ScheduleFileData>>(scheduleFilesFromRepo);

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
        /// An <see cref="IEnumerable{T}"/> of <see cref="ScheduleData"/>
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public IEnumerable<ScheduleData> GetModuleSchedules(int moduleId)
        {
            if (moduleId < 0)
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
                ScheduleData schedule = _mapper.Map<ScheduleData>(scheduleFromRepo);

                IEnumerable<ScheduleFileEntity> scheduleFilesFromRepo =
                    _scheduleFileRepository.GetScheduleFiles(scheduleFromRepo.Id);

                schedule.ScheduleFiles =
                    _mapper.Map<IEnumerable<ScheduleFileData>>(scheduleFilesFromRepo);

                schedule.Module = _mapper.Map<ModuleData>(moduleFromRepo);

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
        /// Otherwise, an instance of <see cref="ScheduleData"/>
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public ScheduleData? GetSchedule(int id)
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

            ScheduleData scheduleModel = _mapper.Map<ScheduleData>(scheduleFromRepo);

            scheduleModel.Module = _mapper.Map<ModuleData>(moduleFromRepo);

            scheduleModel.ScheduleFiles =
                _mapper.Map<IEnumerable<ScheduleFileData>>(scheduleFilesFromRepo);

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
        /// Otherwise, an instance of updated <see cref="ScheduleData"/>
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public ScheduleData? UpdateSchedule(ScheduleData model)
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

            ScheduleData scheduleModel = _mapper.Map<ScheduleData>(scheduleFromRepo);

            scheduleModel.Module = _mapper.Map<ModuleData>(moduleFromRepo);

            scheduleModel.ScheduleFiles =
                _mapper.Map<IEnumerable<ScheduleFileData>>(scheduleFilesFromRepo);

            return scheduleModel;
        }

        public IEnumerable<ScheduleData> GetAll()
        {
            IEnumerable<ScheduleData>? items = _scheduleRepository.GetAll().Select(x => _mapper.Map<ScheduleData>(x));
            return items;
        }

        public ScheduleData? Create(ScheduleData data)
        {
            ScheduleEntity? mappedItem = _mapper.Map<ScheduleEntity>(data);
            int itemId = _scheduleRepository.Create(mappedItem);

            if (itemId == 0)
                return null;

            return GetByKey(itemId);
        }

        public ScheduleData? GetByKey(int key)
        {
            ScheduleEntity? item = _scheduleRepository.GetByKey(key);

            if (item is null)
                return null;

            return _mapper.Map<ScheduleData>(item);
        }

        public ScheduleData? Update(int key, ScheduleData data)
        {
            ScheduleEntity? item = _mapper.Map<ScheduleEntity>(data);
            item.Id = key;

            bool result = _scheduleRepository.Update(item);

            if (!result)
                return null;

            ScheduleData? response = GetByKey(key);

            return response;
        }

        public bool Delete(int key)
        {
            return _scheduleRepository.Delete(_mapper.Map<ScheduleEntity>(GetByKey(key)));
        }
    }
}
