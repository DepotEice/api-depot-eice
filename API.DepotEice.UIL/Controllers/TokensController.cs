﻿using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.DepotEice.UIL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokensController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IUserTokenRepository _userTokenRepository;

        public TokensController(ILogger<TokensController> logger, IMapper mapper, IUserRepository userRepository,
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

            if (userTokenRepository is null)
            {
                throw new ArgumentNullException(nameof(userTokenRepository));
            }

            _logger = logger;
            _mapper = mapper;
            _userRepository = userRepository;
            _userTokenRepository = userTokenRepository;
        }

        /// <summary>
        /// Validate the given token
        /// </summary>
        /// <param name="token">The token to validate</param>
        /// <returns>
        /// Nothing if the token is valid
        /// </returns>
        [HttpGet(nameof(IsValid))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(string))]
        public IActionResult IsValid(string token)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrWhiteSpace(token))
            {
                return BadRequest("The token is required");
            }

            UserTokenEntity? tokenFromRepo = _userTokenRepository
                .GetAll()
                .FirstOrDefault(ut => ut.Value.Equals(token) && ut.ExpirationDate > DateTime.Now);

            if (tokenFromRepo is null)
            {
                return NotFound("Token doesn't exist");
            }

            if (!_userTokenRepository.ApproveToken(tokenFromRepo))
            {
                return BadRequest("Token is used or expired");
            }

            return NoContent();
        }
    }
}
