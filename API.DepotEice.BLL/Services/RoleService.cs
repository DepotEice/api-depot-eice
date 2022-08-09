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
    public class RoleService : IRoleService
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRepository _userRepository;

        public RoleService(ILogger<RoleService> logger, IMapper mapper,
            IRoleRepository roleRepository, IUserRepository userRepository)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (mapper is null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            if (roleRepository is null)
            {
                throw new ArgumentNullException(nameof(roleRepository));
            }

            if (userRepository is null)
            {
                throw new ArgumentNullException(nameof(userRepository));
            }

            _logger = logger;
            _mapper = mapper;
            _roleRepository = roleRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Add a User to a Role
        /// </summary>
        /// <param name="id">
        /// The ID of the Role to which the User must be added
        /// </param>
        /// <param name="userId">
        /// The ID of the User to add
        /// </param>
        /// <returns>
        /// <c>true/c> If the User was successfully added to the Role. <c>false</c> If the Role
        /// does not exist or if the User does not exist or if the operation failed
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool AddUser(string id, string userId)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            RoleEntity? roleFromRepo = _roleRepository.GetByKey(id);

            if (roleFromRepo is null)
            {
                _logger.LogWarning(
                    "{date} - The Role with ID \"{id}\" does not exist in the database!",
                    DateTime.Now, id);

                return false;
            }

            UserEntity? userFromRepo = _userRepository.GetByKey(userId);

            if (userFromRepo is null)
            {
                _logger.LogWarning(
                    "{date} - The User with ID \"{id}\" does not exist in the database!",
                    DateTime.Now, userId);

                return false;
            }

            return _roleRepository.AddUser(id, userId);
        }

        /// <summary>
        /// Create a new Role in the database
        /// </summary>
        /// <param name="model">
        /// The Role to create
        /// </param>
        /// <returns>
        /// <c>null</c> If the creation failed or if the new Role retrieved from database is null.
        /// Otherwise an instance of <see cref="RoleDto"/>
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public RoleDto? CreateRole(RoleDto model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            RoleEntity roleEntity = _mapper.Map<RoleEntity>(model);

            string newId = _roleRepository.Create(roleEntity);

            if (string.IsNullOrEmpty(newId))
            {
                _logger.LogWarning(
                    "{date} - The Role could not be created!",
                    DateTime.Now);

                return null;
            }

            RoleEntity? roleFromRepo = _roleRepository.GetByKey(newId);

            if (roleFromRepo is null)
            {
                _logger.LogError(
                    "{date} - The retrieved Role with the newly created ID \"{id}\" " +
                    "returned null!",
                    DateTime.Now, newId);

                return null;
            }

            RoleDto roleModel = _mapper.Map<RoleDto>(roleFromRepo);

            return roleModel;
        }

        /// <summary>
        /// Delete a Role from the database
        /// </summary>
        /// <param name="id">
        /// The ID of the Role to delete
        /// </param>
        /// <returns>
        /// <c>true</c> If the Role was successfully deleted. <c>false</c> If the Role does not 
        /// exist or if the removal failed
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool DeleteRole(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            RoleEntity? roleFromRepo = _roleRepository.GetByKey(id);

            if (roleFromRepo is null)
            {
                _logger.LogWarning(
                    "{date} - The Role with ID \"{id}\" does not exist in the database!",
                    DateTime.Now, id);

                return false;
            }

            return _roleRepository.Delete(roleFromRepo);
        }

        /// <summary>
        /// Retrieve a Role from the database
        /// </summary>
        /// <param name="id">
        /// The ID of the Role to retrieve
        /// </param>
        /// <returns>
        /// <c>null</c> If the Role does not exist. Otherwise, an instance of 
        /// <see cref="RoleDto"/>
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public RoleDto? GetRole(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            RoleEntity? roleFromRepo = _roleRepository.GetByKey(id);

            if (roleFromRepo is null)
            {
                _logger.LogWarning(
                    "{date} - There is no Role with ID \"{id}\" in the database!",
                    DateTime.Now, id);

                return null;
            }

            RoleDto roleModel = _mapper.Map<RoleDto>(roleFromRepo);

            return roleModel;
        }

        public RoleDto? GetRoleByName(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException(nameof(roleName));
            }

            return _roleRepository.GetAll().SingleOrDefault(r => r.Name.Equals(roleName));
        }

        /// <summary>
        /// Retrieve all Roles from the database
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="RoleDto"/>
        /// </returns>
        public IEnumerable<RoleDto> GetRoles()
        {
            IEnumerable<RoleEntity> rolesFromRepo = _roleRepository.GetAll();

            foreach (RoleEntity roleEntity in rolesFromRepo)
            {
                RoleDto roleModel = _mapper.Map<RoleDto>(roleEntity);

                yield return roleModel;
            }
        }

        /// <summary>
        /// Retrieve all Roles related to a User
        /// </summary>
        /// <param name="userId">
        /// The ID of the User
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="RoleDto"/>
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public IEnumerable<RoleDto> GetUserRoles(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            IEnumerable<RoleEntity> rolesFromRepo = _roleRepository.GetUserRoles(userId);

            foreach (RoleEntity roleEntity in rolesFromRepo)
            {
                RoleDto roleModel = _mapper.Map<RoleDto>(roleEntity);

                yield return roleModel;
            }
        }

        /// <summary>
        /// Remove a User from a Role
        /// </summary>
        /// <param name="id">
        /// The ID of the Role from which the User must be removed
        /// </param>
        /// <param name="userId">
        /// The ID of the User
        /// </param>
        /// <returns>
        /// <c>false</c> If the Role does not exist or if the User does not exist or if the removal
        /// failed. <c>true</c> Otherwise
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool RemoveUser(string id, string userId)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            RoleEntity? roleFromRepo = _roleRepository.GetByKey(id);

            if (roleFromRepo is null)
            {
                _logger.LogWarning(
                    "{date} - The Role with ID \"{id}\" does not exist in the database!",
                    DateTime.Now, id);

                return false;
            }

            UserEntity? userFromRepo = _userRepository.GetByKey(userId);

            if (userFromRepo is null)
            {
                _logger.LogWarning(
                    "{date} - The User with ID \"{id}\" does not exist in the database!",
                    DateTime.Now, userId);

                return false;
            }

            return _roleRepository.RemoveUser(id, userId);
        }

        /// <summary>
        /// Update a Role in the database
        /// </summary>
        /// <param name="model">
        /// The Role to update
        /// </param>
        /// <returns>
        /// <c>null</c> If the update failed or if the retriaval of the updated Role is null.
        /// Otherwise, an instance of <see cref="RoleDto"/>
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public RoleDto? UpdateRole(RoleDto model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            RoleEntity roleToUpdate = _mapper.Map<RoleEntity>(model);

            if (!_roleRepository.Update(roleToUpdate))
            {
                _logger.LogWarning(
                    "{date} - The Role with ID \"{id}\" could no be updated!",
                    DateTime.Now, roleToUpdate.Id);

                return null;
            }

            RoleEntity? roleFromRepo = _roleRepository.GetByKey(model.Id);

            if (roleFromRepo is null)
            {
                _logger.LogError(
                    "{date} - The retrieval of the Role with ID \"{id}\" that has just been " +
                    "updated, returned null!",
                    DateTime.Now, model.Id);

                return null;
            }

            RoleDto roleModel = _mapper.Map<RoleDto>(roleFromRepo);

            return roleModel;
        }
    }
}
