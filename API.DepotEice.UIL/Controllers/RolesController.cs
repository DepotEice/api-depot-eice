using API.DepotEice.DAL.IRepositories;
using API.DepotEice.UIL.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.DepotEice.UIL.Controllers
{
    /// <summary>
    /// The controller for the roles
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserManager _userManager;

        /// <summary>
        /// Instanciate the Roles controller
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        /// <param name="roleRepository"></param>
        /// <param name="userManager"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public RolesController(ILogger<RolesController> logger, IMapper mapper, IRoleRepository roleRepository,
            IUserManager userManager)
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

            if (userManager is null)
            {
                throw new ArgumentNullException(nameof(IUserManager));
            }

            _logger = logger;
            _mapper = mapper;
            _roleRepository = roleRepository;
            _userManager = userManager;
        }

        /// <summary>
        /// Verify if the current user has the required role
        /// </summary>
        /// <param name="roleName">The name of the role on which the verification is passed</param>
        /// <returns>
        /// BadRequest if the role name is null or empty or if an error occured during the request
        /// Unauthorized if the current user's ID is null or empty
        /// </returns>
        [HttpGet("HasRole/{roleName}")]
        public IActionResult HasRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return BadRequest("Please provide a valid roleName");
            }

            try
            {
                string? userId = _userManager.GetCurrentUserId;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var roles = _roleRepository.GetUserRoles(userId);

                return Ok(roles.Any(r => r.Name.ToUpper().Equals(roleName.ToUpper())));
            }
            catch (Exception ex)
            {
                _logger.LogError($"{DateTime.Now} - An exception was thrown when trying to retrieve the role.\"" +
                    $"{ex.Message}\n{ex.StackTrace}");

                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Verify if a certain user has a role
        /// </summary>
        /// <param name="roleName">The name of the role on which the verification is done</param>
        /// <param name="userId">The ID of the User on which the verification is done</param>
        /// <returns>
        /// Bad request if the user ID or the role name is null or empty or if an exception occured
        /// </returns>
        [HttpGet("HasRole/{roleName}/{userId}")]
        public IActionResult HasRole(string roleName, string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Please provide a valid user id");
            }

            if (string.IsNullOrEmpty(roleName))
            {
                return BadRequest("Please provide a valid roleName");
            }

            try
            {
                var roles = _roleRepository.GetUserRoles(userId);

                return Ok(roles.Any(r => r.Name.ToUpper().Equals(roleName.ToUpper())));
            }
            catch (Exception ex)
            {
                _logger.LogError($"{DateTime.Now} - An exception was thrown when trying to retrieve the role.\"" +
                    $"{ex.Message}\n{ex.StackTrace}");

                return BadRequest(ex.Message);
            }
        }
    }
}
