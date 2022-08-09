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
    public class UserService : IUserService
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public UserService(ILogger<UserService> logger, IMapper mapper,
            IUserRepository userRepository, IRoleRepository roleRepository)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (mapper is null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            if (userRepository is null)
            {
                throw new ArgumentNullException(nameof(userRepository));
            }

            if (roleRepository is null)
            {
                throw new ArgumentNullException(nameof(roleRepository));
            }

            _logger = logger;
            _mapper = mapper;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public bool ActivateUser(string id, bool isActive)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            UserEntity? userFromRepo = _userRepository.GetByKey(id);

            if (userFromRepo is null)
            {
                _logger.LogWarning(
                    "{date} - There is no User with ID \"{id}\"!",
                    DateTime.Now, id);

                return false;
            }

            return _userRepository.ActivateUser(id, isActive);
        }

        public UserDto? CreateUser(UserDto user)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            UserEntity userToCreate = _mapper.Map<UserEntity>(user);

            string newId = _userRepository.Create(userToCreate);

            if (string.IsNullOrEmpty(newId))
            {
                _logger.LogWarning(
                    "{date} - The User could not be created!",
                    DateTime.Now);

                return null;
            }

            UserEntity? userFromRepo = _userRepository.GetByKey(newId);

            if (userFromRepo is null)
            {
                _logger.LogError(
                    "{date} - The retrieval of the newly created User with ID \"{id}\" " +
                    "returned null!",
                    DateTime.Now, newId);

                return null;
            }

            UserDto userModel = _mapper.Map<UserDto>(userFromRepo);



            return userModel;
        }

        public bool DeleteUser(string id)
        {
            throw new NotImplementedException();
        }

        public UserDto? GetUser(string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<UserDto> GetUsers()
        {
            var repoUsers = _userRepository.GetAll();
            var mapperUsers = _mapper.Map<IEnumerable<UserDto>>(repoUsers);
            return mapperUsers;
        }

        public bool UpdatePassword(string id, string oldPassword, string newPassword, string salt)
        {
            throw new NotImplementedException();
        }

        public bool UserExist(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            return _userRepository.GetByKey(id) is not null;
        }

        public bool EmailExist(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            return _userRepository.GetAll().Any(u => u.NormalizedEmail.Equals(email.ToUpper()));
        }
    }
}
