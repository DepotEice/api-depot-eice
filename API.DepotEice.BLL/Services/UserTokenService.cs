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
    public class UserTokenService : IUserTokenService
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IUserTokenRepository _userTokenRepository;
        private readonly IUserRepository _userRepository;

        public UserTokenService(ILogger<UserTokenService> logger, IMapper mapper,
            IUserTokenRepository userTokenRepository, IUserRepository userRepository)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (mapper is null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            if (userTokenRepository is null)
            {
                throw new ArgumentNullException(nameof(userTokenRepository));
            }

            if (userRepository is null)
            {
                throw new ArgumentNullException(nameof(userRepository));
            }

            _logger = logger;
            _mapper = mapper;
            _userTokenRepository = userTokenRepository;
            _userRepository = userRepository;
        }

        public UserTokenDto? CreateUserToken(UserTokenDto model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            UserTokenEntity tokenToCreate = _mapper.Map<UserTokenEntity>(model);

            string createdTokenId = _userTokenRepository.Create(tokenToCreate);

            UserTokenEntity? tokenFromRepo = _userTokenRepository.GetByKey(createdTokenId);

            if (tokenFromRepo is null)
            {
                _logger.LogError(
                    "{date} - Retrieving User Token with ID \"{id}\" returned null!",
                    DateTime.Now, createdTokenId);

                return null;
            }

            UserTokenDto createdToken = _mapper.Map<UserTokenDto>(tokenFromRepo);

            UserEntity? userFromRepo = _userRepository.GetByKey(tokenFromRepo.UserId);

            if (userFromRepo is null)
            {
                _logger.LogError(
                    "{date} - The retrieval of a User with ID \"{userId}\" related to the " +
                    "newly created with ID \"{tokenId}\" returned a null!",
                    DateTime.Now, tokenFromRepo.UserId, tokenFromRepo.Id);

                return null;
            }

            createdToken.User = _mapper.Map<UserDto>(userFromRepo);

            return createdToken;
        }

        public bool DeleteUserToken(string id)
        {
            throw new NotImplementedException();
        }

        public UserTokenDto? GetUserToken(string tokenType, DateTime deliveryDate, string userId)
        {
            if (string.IsNullOrEmpty(tokenType))
            {
                throw new ArgumentNullException(nameof(tokenType));
            }

            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            UserTokenEntity? userTokenFromRepo = _userTokenRepository
                .GetUserTokens(userId)
                .SingleOrDefault(ut =>
                    (ut.DeliveryDateTime > deliveryDate) &&
                    ut.Type.Equals(UserTokenTypes.EMAIL_CONFIRMATION_TOKEN));

            if (userTokenFromRepo is null)
            {
                _logger.LogWarning(
                    "{date} - There is no user token with type \"{tokenType}\" delivered " +
                    "at \"{deliveryDate}\"!",
                    DateTime.Now, tokenType, deliveryDate);

                return null;
            }

            UserTokenDto userToken = _mapper.Map<usertokendto>(userTokenFromRepo);

            return userToken;
        }

        public IEnumerable<UserTokenDto> GetUserTokens(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            IEnumerable<UserTokenEntity> userTokensFromRepo =
                _userTokenRepository.GetUserTokens(id);

            foreach (UserTokenEntity userTokenFromRepo in userTokensFromRepo)
            {
                UserTokenDto userToken = _mapper.Map<UserTokenDto>(userTokenFromRepo);

                yield return userToken;
            }
        }
    }
}
