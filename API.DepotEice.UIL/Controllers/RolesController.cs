using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.UIL.AuthorizationAttributes;
using API.DepotEice.UIL.Interfaces;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using static API.DepotEice.UIL.Data.RolesData;

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
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Instanciate the Roles controller
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        /// <param name="roleRepository"></param>
        /// <param name="userManager"></param>
        /// <param name="userRepository"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public RolesController(ILogger<RolesController> logger, IMapper mapper, IRoleRepository roleRepository,
            IUserManager userManager, IUserRepository userRepository)
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
                throw new ArgumentNullException(nameof(userManager));
            }

            if (userRepository is null)
            {
                throw new ArgumentNullException(nameof(userRepository));
            }

            _logger = logger;
            _mapper = mapper;
            _roleRepository = roleRepository;
            _userManager = userManager;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Get the authenticated user's roles
        /// </summary>
        /// <returns>
        /// <see cref="StatusCodes.Status200OK"/> If the operation is successful
        /// <see cref="StatusCodes.Status401Unauthorized"/> If the user is not authenticated
        /// </returns>
        [HttpGet("Me")]
        public IActionResult Me()
        {
            try
            {
                string? userId = _userManager.GetCurrentUserId;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("The user must be authenticated");
                }

                IEnumerable<RoleEntity> rolesFromRepo = _roleRepository.GetUserRoles(userId);

                IEnumerable<RoleModel> userRoles = _mapper.Map<IEnumerable<RoleModel>>(rolesFromRepo);

                return Ok(userRoles);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{DateTime.Now} - An exception was thrown during {nameof(Me)}.\"" +
                    $"{ex.Message}\n{ex.StackTrace}");
#if DEBUG
                return BadRequest(ex.Message);
#else
            return BadRequest("An error occurred while trying to user roles (/Me), please contact the administrator");
#endif
            }
        }

        /// <summary>
        /// Get all user's roles
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>
        /// <see cref="StatusCodes.Status200OK"/> If the operation is successful
        /// <see cref="StatusCodes.Status404NotFound"/> If the user doesn't exist
        /// <see cref="StatusCodes.Status400BadRequest"/> If an error occurred
        /// </returns>
        [HasRoleAuthorize(RolesEnum.DIRECTION, AndAbove = false)]
        [HttpGet("UserRoles/{userId}")]
        public IActionResult GetUserRoles(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("The provided user id is null or empty");
            }

            try
            {
                UserEntity? userFromRepo = _userRepository.GetByKey(userId);

                if (userFromRepo is null)
                {
                    return NotFound($"There is no user with ID \"{userId}\"");
                }

                IEnumerable<RoleEntity> rolesFromRepo = _roleRepository.GetUserRoles(userId);

                IEnumerable<RoleModel> userRoles = _mapper.Map<IEnumerable<RoleModel>>(rolesFromRepo);

                return Ok(userRoles);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{DateTime.Now} - An exception was thrown during {nameof(GetUserRoles)}.\"" +
                    $"{ex.Message}\n{ex.StackTrace}");
#if DEBUG
                return BadRequest(ex.Message);
#else
            return BadRequest("An error occurred while trying to retrieve user roles (/UserRoles), please contact " +
                $"the administrator");
#endif
            }
        }

        /// <summary>
        /// Get all the available roles
        /// </summary>
        /// <returns>
        /// <see cref="StatusCodes.Status200OK"/> If the operation is successful
        /// <see cref="StatusCodes.Status400BadRequest"/> If an error occurred
        /// </returns>
        [HasRoleAuthorize(RolesEnum.DIRECTION, AndAbove = false)]
        [HttpGet]
        public IActionResult GetRoles()
        {
            try
            {
                IEnumerable<RoleEntity> rolesFromRepo = _roleRepository.GetAll();

                IEnumerable<RoleModel> roles = _mapper.Map<IEnumerable<RoleModel>>(rolesFromRepo);

                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{DateTime.Now} - An exception was thrown during {nameof(GetRoles)}.\"" +
                    $"{ex.Message}\n{ex.StackTrace}");
#if DEBUG
                return BadRequest(ex.Message);
#else
            return BadRequest("An error occurred while trying to get roles, please contact the administrator");
#endif
            }
        }

        /// <summary>
        /// Create a new role
        /// </summary>
        /// <param name="role">The role form</param>
        /// <returns>
        /// <see cref="StatusCodes.Status200OK"/> If the creation was successful
        /// <see cref="StatusCodes.Status404NotFound"/> If the created role couldn't be found
        /// <see cref="StatusCodes.Status400BadRequest"/> If an error occurred during the proces
        /// </returns>
        [HasRoleAuthorize(RolesEnum.DIRECTION, AndAbove = false)]
        [HttpPost]
        public IActionResult CreateRole(RoleForm role)
        {
            if (role is null)
            {
                return BadRequest($"The body content of the request is null");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                RoleEntity roleToCreate = _mapper.Map<RoleEntity>(role);

                string createdRoleId = _roleRepository.Create(roleToCreate);

                if (string.IsNullOrEmpty(createdRoleId))
                {
                    return BadRequest($"The role creation failed");
                }

                RoleEntity? createdRole = _roleRepository.GetByKey(createdRoleId);

                if (createdRole is null)
                {
                    return NotFound($"The newly created role \"{role.Name}\" couldn't be found");
                }

                RoleModel roleModel = _mapper.Map<RoleModel>(createdRole);

                return Ok(roleModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{DateTime.Now} - An exception was thrown during {nameof(CreateRole)}.\"" +
                    $"{ex.Message}\n{ex.StackTrace}");
#if DEBUG
                return BadRequest(ex.Message);
#else
            return BadRequest("An error occurred while trying to create a role, please contact the administrator");
#endif

            }
        }

        /// <summary>
        /// Update the given role
        /// </summary>
        /// <param name="roleId">The ID of the role to update</param>
        /// <param name="role">The role form</param>
        /// <returns>
        /// <see cref="StatusCodes.Status200OK"/> If the update is successful
        /// <see cref="StatusCodes.Status404NotFound"/> If the no role with the given ID could be found
        /// <see cref="StatusCodes.Status400BadRequest"/> If the operation failed
        /// </returns>
        [HasRoleAuthorize(RolesEnum.DIRECTION, AndAbove = false)]
        [HttpPut("{roleId}")]
        public IActionResult UpdateRole(string roleId, [FromBody] RoleForm role)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                return BadRequest("The provided role ID is null or empty");
            }

            if (role is null)
            {
                return BadRequest("The role form is null");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                RoleEntity? roleFromRepo = _roleRepository.GetByKey(roleId);

                if (roleFromRepo is null)
                {
                    return NotFound($"There is no role with ID \"{roleId}\"");
                }

                roleFromRepo = _mapper.Map<RoleEntity>(role);

                if (!_roleRepository.Update(roleId, roleFromRepo))
                {
                    return BadRequest("The update failed");
                }

                roleFromRepo = _roleRepository.GetByKey(roleId);

                RoleModel roleModel = _mapper.Map<RoleModel>(roleFromRepo);

                return Ok(roleModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{DateTime.Now} - An exception was thrown during {nameof(UpdateRole)}.\"" +
                    $"{ex.Message}\n{ex.StackTrace}");
#if DEBUG
                return BadRequest(ex.Message);
#else
            return BadRequest("An error occurred while trying to update a role, please contact the administrator");
#endif
            }
        }

        /// <summary>
        /// Assign a role to a user
        /// </summary>
        /// <param name="roleId">The role ID</param>
        /// <param name="userId">The user ID</param>
        /// <returns>
        /// <see cref="StatusCodes.Status200OK"/> If the assignment is successful
        /// <see cref="StatusCodes.Status404NotFound"/> If the role or the user could not be found
        /// <see cref="StatusCodes.Status400BadRequest"/> If the operation was unsucessful
        /// </returns>
        [HasRoleAuthorize(RolesEnum.DIRECTION, AndAbove = false)]
        [HttpPost("{roleId}/User/{userId}")]
        public IActionResult AssignRole(string roleId, string userId)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                return BadRequest("The role ID is null or empty");
            }

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("The user ID is null or empty");
            }

            try
            {
                RoleEntity? roleFromRepo = _roleRepository.GetByKey(roleId);

                if (roleFromRepo is null)
                {
                    return NotFound($"There is no role with the ID \"{roleId}\"");
                }

                UserEntity? userFromRepo = _userRepository.GetByKey(userId);

                if (userFromRepo is null)
                {
                    return NotFound($"There is no user with the ID \"{userId}\"");
                }

                if (!_roleRepository.AddUser(roleId, userId))
                {
                    return BadRequest("The role couldn't be assigned to the user");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{DateTime.Now} - An exception was thrown during {nameof(AssignRole)}.\"" +
                    $"{ex.Message}\n{ex.StackTrace}");
#if DEBUG
                return BadRequest(ex.Message);
#else
            return BadRequest("An error occurred while trying to assign a role to a user, please contact the " +
                $"administrator");
#endif
            }
        }
    }
}
