using API.DepotEice.BLL.IServices;
using API.DepotEice.BLL.Models;
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

        public UserTokenModel? CreateUserToken(UserTokenModel model)
        {
            throw new NotImplementedException();
        }

        public bool DeleteUserToken(string id)
        {
            throw new NotImplementedException();
        }

        public UserTokenModel? GetUserToken(string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<UserTokenModel> GetUserTokens(string id)
        {
            throw new NotImplementedException();
        }
    }
}
