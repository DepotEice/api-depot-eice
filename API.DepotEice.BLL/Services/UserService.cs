using API.DepotEice.BLL.IServices;
using API.DepotEice.BLL.Models;
using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
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
        private readonly IUserTokenRepository _userTokenRepository;

        public UserService(ILogger<UserService> logger, IMapper mapper,
            IUserRepository userRepository, IRoleRepository roleRepository,
            IUserTokenRepository userTokenRepository)
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

            if (userTokenRepository is null)
            {
                throw new ArgumentNullException(nameof(userTokenRepository));
            }

            _logger = logger;
            _mapper = mapper;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userTokenRepository = userTokenRepository;
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

            string createdUserTokenID = _userTokenRepository.Create(new UserTokenEntity
                (
                    UserTokenTypes.EMAIL_CONFIRMATION_TOKEN,
                    DateTime.Now.AddDays(2),
                    userFromRepo.Id,
                    userFromRepo.SecurityStamp
                ));

            if (string.IsNullOrEmpty(createdUserTokenID))
            {
                _logger.LogWarning(
                    "{date} - The UserToken creation failed!",
                    DateTime.Now);

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

        /// <summary>
        /// Verify if the User credentials are correct. Once it is done, create a new JWT Token
        /// </summary>
        /// <param name="email">
        /// User's email address
        /// </param>
        /// <param name="password">
        /// User's password
        /// </param>
        /// <param name="secret"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public string LogIn(string email, string password, JwtTokenDto tokenDto)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            UserEntity? userFromRepo = _userRepository.LogIn(email, password);

            if (userFromRepo is null)
            {
                return string.Empty;
            }

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id),
                new Claim(ClaimTypes.Email, userFromRepo.Email)
            };

            IEnumerable<Claim> roleClaims = _roleRepository
                .GetUserRoles(userFromRepo.Id)
                .Select(ur => new Claim(ClaimTypes.Role, ur.Name));

            claims.AddRange(roleClaims);

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenDto.Secret));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.Sha256);

            DateTime expirationDate = DateTime.Now.AddDays(15);

            JwtSecurityToken jwtToken = new JwtSecurityToken
                (
                    issuer: tokenDto.Issuer,
                    audience: tokenDto.Audience,
                    claims: claims,
                    expires: expirationDate,
                    signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
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
