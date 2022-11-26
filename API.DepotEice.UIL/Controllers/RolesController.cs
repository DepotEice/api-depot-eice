using API.DepotEice.DAL.IRepositories;
using API.DepotEice.UIL.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.DepotEice.UIL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserManager _userManager;

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

                return Ok(roles.SingleOrDefault(r => r.Name.ToUpper().Equals(roleName.ToUpper())) is not null);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{DateTime.Now} - An exception was thrown when trying to retrieve the role.\"" +
                    $"{ex.Message}\n{ex.StackTrace}");

                return BadRequest(ex.Message);
            }
        }

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

                return Ok(roles.SingleOrDefault(r => r.Name.ToUpper().Equals(roleName.ToUpper())) is not null);
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
