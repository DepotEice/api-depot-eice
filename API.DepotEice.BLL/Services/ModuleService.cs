using API.DepotEice.BLL.IServices;
using API.DepotEice.BLL.Models;
using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace API.DepotEice.BLL.Services
{
    public class ModuleService : IModuleService
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IModuleRepository _moduleRepository;
        private readonly IUserRepository _userRepository;

        public ModuleService(ILogger<ModuleService> logger, IMapper mapper,
            IModuleRepository moduleRepository, IUserRepository userRepository)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (mapper is null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            if (moduleRepository is null)
            {
                throw new ArgumentNullException(nameof(moduleRepository));
            }

            if (userRepository is null)
            {
                throw new ArgumentNullException(nameof(userRepository));
            }

            _logger = logger;
            _mapper = mapper;
            _moduleRepository = moduleRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Add a user to a module and set its acceptance flag
        /// </summary>
        /// <param name="id">
        /// The ID of the Module
        /// </param>
        /// <param name="userId">
        /// The ID of the User
        /// </param>
        /// <param name="isAccepted">
        /// The acceptance flag
        /// </param>
        /// <returns>
        /// <c>true</c> If the user has been correctly added. <c>false</c> Otherwise
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public bool AddUser(int id, string userId, bool isAccepted)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            return _moduleRepository.AddUser(id, userId, isAccepted);
        }

        /// <summary>
        /// Create a new module in the database
        /// </summary>
        /// <param name="model">
        /// The module to create
        /// </param>
        /// <returns>
        /// <c>null</c> If the module creation failed. Otherwise an instance of 
        /// <see cref="ModuleData"/> of the newly created Module
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public ModuleData? CreateModule(ModuleData model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            ModuleEntity? moduleEntity = _mapper.Map<ModuleEntity>(model);

            int newID = _moduleRepository.Create(moduleEntity);

            if (newID <= 0)
            {
                _logger.LogError(
                    "{date} - The newly created module's ID is smaller or equals to 0!",
                    DateTime.Now);

                return null;
            }

            ModuleEntity? moduleFromRepo = _moduleRepository.GetByKey(newID);

            if (moduleFromRepo is null)
            {
                _logger.LogWarning(
                    "{date} - The retrieved module with the new ID : \"{id}\" is null!",
                    DateTime.Now, newID);

                return null;
            }

            ModuleData moduleModel = _mapper.Map<ModuleData>(moduleFromRepo);

            moduleModel.Users = _mapper
                .Map<IEnumerable<UserDto>>(_userRepository.GetModuleUsers(newID));

            return moduleModel;
        }

        /// <summary>
        /// Delete a module from the database
        /// </summary>
        /// <param name="id">
        /// The module's ID
        /// </param>
        /// <returns>
        /// <c>true</c> If the module is successfully deleted. <c>false</c> If the module does not
        /// exist or if the deletion failed
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public bool DeleteModule(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            ModuleEntity? moduleFromRepo = _moduleRepository.GetByKey(id);

            if (moduleFromRepo is null)
            {
                _logger.LogWarning(
                    "{date} - The module with ID \"{id}\" does not exist!",
                    DateTime.Now, id);

                return false;
            }

            return _moduleRepository.Delete(moduleFromRepo);
        }

        /// <summary>
        /// Retrieve a module from the database
        /// </summary>
        /// <param name="id">
        /// The ID of the module
        /// </param>
        /// <returns>
        /// <c>null</c> If no module matches <paramref name="id"/>. Otherwise, an instance of 
        /// <see cref="ModuleData"/>
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public ModuleData? GetModule(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            ModuleEntity? moduleFromRepo = _moduleRepository.GetByKey(id);

            if (moduleFromRepo is null)
            {
                _logger.LogWarning(
                    "{date} - The module with ID \"{id}\" does not exist!",
                    DateTime.Now, id);

                return null;
            }

            ModuleData moduleModel = _mapper.Map<ModuleData>(moduleFromRepo);

            // Handle them from UIL by authorization.
            // moduleModel.Users = _mapper.Map<IEnumerable<UserModel>>(_userRepository.GetModuleUsers(id));

            return moduleModel;
        }

        /// <summary>
        /// Retrieve all modules from the database
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="UserDto"/>
        /// </returns>
        public IEnumerable<ModuleData> GetModules()
        {
            IEnumerable<ModuleEntity> modulesFromRepo = _moduleRepository.GetAll();

            return _moduleRepository.GetAll().Select(x => _mapper.Map<ModuleData>(x));

            // TODO - Modules modifié pour test

            //foreach (ModuleEntity moduleFromRepo in modulesFromRepo)
            //{
            //    ModuleModel moduleModel = _mapper.Map<ModuleModel>(moduleFromRepo);

            //    IEnumerable<UserEntity> moduleUsers =
            //        _userRepository.GetModuleUsers(moduleModel.Id);

            //    moduleModel.Users = _mapper.Map<IEnumerable<UserModel>>(moduleUsers);

            //    yield return moduleModel;
            //}
        }

        /// <summary>
        /// Retrieve all modules linked to the user
        /// </summary>
        /// <param name="userId">
        /// The ID of the user
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="ModuleData"/>
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public IEnumerable<ModuleData> GetUserModules(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            IEnumerable<ModuleEntity> modulesFromRepo = _moduleRepository.GetUserModules(userId);

            foreach (ModuleEntity moduleFromRepo in modulesFromRepo)
            {
                ModuleData moduleModel = _mapper.Map<ModuleData>(moduleFromRepo);

                IEnumerable<UserEntity> moduleUsers =
                    _userRepository.GetModuleUsers(moduleModel.Id);

                moduleModel.Users = _mapper.Map<IEnumerable<UserDto>>(moduleUsers);

                yield return moduleModel;
            }
        }

        /// <summary>
        /// Remove a user from a module
        /// </summary>
        /// <param name="id">
        /// The ID of the module
        /// </param>
        /// <param name="userId">
        /// The ID of the user to remove
        /// </param>
        /// <returns>
        /// <c>true</c> If the user has correctly been removed. <c>false</c> Otherwise
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public bool RemoveUser(int id, string userId)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            return _moduleRepository.RemoveUser(id, userId);
        }

        /// <summary>
        /// Update Module <paramref name="model"/> in the database
        /// </summary>
        /// <param name="model">
        /// The Module to update
        /// </param>
        /// <returns>
        /// <c>null</c> If the module does not exist or if the update failed. Otherwise an instance
        /// of <see cref="ModuleData"/>
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public ModuleData? UpdateModule(ModuleData model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            ModuleEntity? moduleToUpdate = _moduleRepository.GetByKey(model.Id);

            if (moduleToUpdate is null)
            {
                _logger.LogWarning(
                    "{date} - The retrieved module from the database with ID \"{id}\" " +
                    "is null!",
                    DateTime.Now, model.Id);

                return null;
            }

            _mapper.Map(model, moduleToUpdate);

            if (!_moduleRepository.Update(moduleToUpdate))
            {
                _logger.LogWarning(
                    "{date} - The Module with ID \"{id}\" could not be updated!",
                    DateTime.Now, model.Id);

                return null;
            }

            ModuleEntity? moduleFromRepo = _moduleRepository.GetByKey(model.Id);

            if (moduleFromRepo is null)
            {
                _logger.LogError(
                    "{date} - The module with ID \"{id}\" could not be retrieved from the " +
                    "database!",
                    DateTime.Now, model.Id);

                return null;
            }

            ModuleData moduleModel = _mapper.Map<ModuleData>(moduleFromRepo);

            moduleModel.Users = _mapper
                .Map<IEnumerable<UserDto>>(_userRepository.GetModuleUsers(model.Id));

            return moduleModel;
        }

        public IEnumerable<ModuleData> GetAll()
        {
            IEnumerable<ModuleData> modules = _moduleRepository.GetAll().Select(x => _mapper.Map<ModuleData>(x));
            return modules;
        }

        public ModuleData? Create(ModuleData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            ModuleEntity? mappedData = _mapper.Map<ModuleEntity>(data);
            int moduleId = _moduleRepository.Create(mappedData);

            if (moduleId == 0)
            {
                return null;
            }

            ModuleData? retrievedItem = GetByKey(moduleId);

            if (retrievedItem is null)
            {
                return null;
            }

            return retrievedItem;
        }

        public ModuleData? GetByKey(int key)
        {
            if (key == 0)
            {
                return null;
            }

            ModuleEntity? item = _moduleRepository.GetByKey(key);
            ModuleData? mappedItem = _mapper.Map<ModuleData>(item);
            return mappedItem;
        }

        public ModuleData? Update(int key, ModuleData data)
        {
            data.Id = key;
            var mappedItem = _mapper.Map<ModuleEntity>(data);
            bool success = _moduleRepository.Update(mappedItem);
            if (!success) return null;
            return GetByKey(key);
        }

        public bool Delete(int key)
        {
            ModuleData? item = GetByKey(key);
            ModuleEntity? mappedItem = _mapper.Map<ModuleEntity>(item);
            bool result = _moduleRepository.Delete(mappedItem);
            return result;
        }

        public bool StudentApply(string userId, int moduleId)
        {
            bool output = false;
            ModuleData? module = GetByKey(moduleId);

            if (module == null) return output;

            bool result = _moduleRepository.StuddentApply(userId, moduleId);

            return result;
        }

        public bool DeleteStudentFromModule(string userId, int moduleId)
        {
            return _moduleRepository.DeleteStudentFromModule(userId, moduleId); ;
        }

        public bool StudentAcceptExempt(string userId, int moduleId, bool decision)
        {
            return _moduleRepository.StudentAcceptExempt(userId, moduleId, decision);
        }

        public IEnumerable<UserDto> GetModuleStudents(int moduleId)
        {
            IEnumerable<UserDto> students = _moduleRepository.GetModuleStudents(moduleId).Select(x => _mapper.Map<UserDto>(x));
            return students;
        }
    }
}
