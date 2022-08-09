using API.DepotEice.BLL.IServices;
using API.DepotEice.BLL.Models;
using API.DepotEice.UIL.Data;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.DepotEice.UIL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IUserTokenService _userTokenService;

        public AuthController(ILogger<AuthController> logger, IMapper mapper,
            IUserService userService, IRoleService roleService, IUserTokenService userTokenService)
        {
            _logger = logger;
            _mapper = mapper;
            _userService = userService;
            _roleService = roleService;
            _userTokenService = userTokenService;
        }

        /// <summary>
        /// If user's credentials are correct, sign in the user by generating a JWT Token
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// <see cref="StatusCodes.Status200OK"/> If the user credentials are correct and a JWT 
        /// token was generated
        /// <para />
        /// <see cref="StatusCodes.Status400BadRequest"/> If an error occured during authentication
        /// </returns>
        [HttpPost(nameof(SignIn))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult SignIn([FromBody] LoginForm form)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.ValidationState);
            }

#if DEBUG
            JwtTokenDto jwtToken = new JwtTokenDto("issuer", "audience", "secret", 1);
#else
            JwtTokenDto jwtToken = new JwtTokenDto();
#endif

            string token = _userService.LogIn(form.Email, form.Password, jwtToken);

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning(
                    "{date} - The generated token is an empty string!",
                    DateTime.Now);

                return BadRequest();
            }

            _logger.LogInformation(
                "{date} - The JWT Token has been successfully generated : \"{token}\"!",
                DateTime.Now, token);

            return Ok(token);
        }

        /// <summary>
        /// Register a new User
        /// </summary>
        /// <param name="form">
        /// The user form for registration
        /// </param>
        /// <returns>
        /// <see cref="StatusCodes.Status200OK"/> If the user was correctly created.
        /// <para/>
        /// <see cref="StatusCodes.Status400BadRequest"/> If the email is already used or if the 
        /// user creation failed.
        /// <para/>
        /// <see cref="StatusCodes.Status204NoContent"/> If The role Guest could not be created or
        /// if the user couldn't be added to the role
        /// </returns>
        [HttpPost(nameof(SignUp))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult SignUp([FromBody] RegisterForm form)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.ValidationState);
            }

            if (_userService.EmailExist(form.Email))
            {
                return BadRequest("There is already an account with this email!");
            }

            UserDto? createdUser = _userService.CreateUser(_mapper.Map<UserDto>(form));

            if (createdUser is null)
            {
                return BadRequest("User creation failed");
            }

            RoleDto? guestRole = _roleService.GetRoleByName(RolesData.GUEST_ROLE);

            if (guestRole is null)
            {
                guestRole = _roleService.CreateRole(new RoleDto()
                {
                    Name = RolesData.GUEST_ROLE
                });

                if (guestRole is null)
                {
                    return NoContent();
                }
            }

            if (!_roleService.AddUser(createdUser.Id, guestRole.Id))
            {
                // TODO : Return an error message with explanation of what happened
                return NoContent();
            }

            // TODO : Send a activation email

            UserModel user = _mapper.Map<UserModel>(createdUser);

            return Ok(user);
        }

        [HttpPost(nameof(Activate))]
        public IActionResult Activate(string id, string token)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(id);
            }

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(token);
            }

            return Ok();
        }
    }
}
