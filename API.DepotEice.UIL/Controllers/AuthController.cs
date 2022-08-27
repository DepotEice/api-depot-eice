using API.DepotEice.BLL;
using API.DepotEice.BLL.Dtos;
using API.DepotEice.BLL.IServices;
using API.DepotEice.UIL.Data;
using API.DepotEice.UIL.IManagers;
using API.DepotEice.UIL.Managers;
using API.DepotEice.UIL.Mapper;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using AutoMapper;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace API.DepotEice.UIL.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly ITokenManager _tokenManager;
    private readonly IAuthService _authService;
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly IUserTokenService _userTokenService;

    public AuthController(
        ILogger<AuthController> logger,
        IMapper mapper,
        IConfiguration configuration,
        ITokenManager tokenManager,
        IAuthService authService,
        IUserService userService,
        IRoleService roleService,
        IUserTokenService userTokenService)
    {
        _logger = logger;
        _mapper = mapper;
        _configuration = configuration;
        _tokenManager = tokenManager;
        _authService = authService;
        _userService = userService;
        _roleService = roleService;
        _userTokenService = userTokenService;
    }

    [HttpPost(nameof(Login))]
    public IActionResult Login([FromBody] LoginForm form)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            //LoggedInUserModel? user = _authService.SignIn(form.Email, form.Password)?.ToUil();
            if (!_userService.EmailExist(form.Email))
                return NotFound("No account found with this email");

            LoggedInUserModel? user2 = _authService.SignIn(form.Email, form.Password, _configuration.GetValue<string>("AppSettings:Secret"))?.ToUil();

            // - récuperer l'utilisateur de la base de données,
            LoggedInUserModel? user = new()
            {
                Id = Guid.NewGuid().ToString(),
                Email = "test@gmail.com",
                ProfilePicture = null,
                FirstName = "John",
                LastName = "Price",
                BirthDate = DateTime.Now,
                CreatedAt = DateTime.Now,
                UpdatedAt = null,
                DeletedAt = null,
            };

            if (user == null)
                return BadRequest("The email or password is wrong ! Please try again.");

            TokenModel token = new() { Token = _tokenManager.GenerateJWT(user) };

            return Ok(token);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost(nameof(Register))]
    public IActionResult Register([FromBody] RegisterForm form)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (_userService.EmailExist(form.Email))
            return BadRequest("There is already an account with this email!");

        try
        {
            bool response = _authService.SingUp(form.ToBll());
            if (!response)
                return BadRequest("Please try with another mail or contact the administration");
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
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
            return BadRequest(ModelState.ValidationState);

#if DEBUG
        JwtTokenDto jwtToken = new("issuer", "audience", "secret", 1);
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
    /// The user form for registration in json format
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
            return BadRequest(ModelState.ValidationState);

        if (_userService.EmailExist(form.Email))
            return BadRequest("There is already an account with this email!");

        UserDto? createdUser = _userService.CreateUser(_mapper.Map<UserDto>(form));

        if (createdUser is null)
            return BadRequest("User creation failed");

        RoleDto? guestRole = _roleService.GetRoleByName(RolesData.GUEST_ROLE);

        if (guestRole is null)
        {
            guestRole = _roleService.CreateRole(new RoleDto()
            {
                Name = RolesData.GUEST_ROLE
            });

            if (guestRole is null)
                return NoContent();
        }

        if (!_roleService.AddUser(guestRole.Id, createdUser.Id))
        {
            // TODO : Return an error message with explanation of what happened
            return NoContent();
        }

        UserTokenDto? userTokenDto = _userTokenService.GetUserToken
            (
                UserTokenTypes.EMAIL_CONFIRMATION_TOKEN,
                createdUser.Id
            );

        if (userTokenDto is null)
        {
            _logger.LogError(
                "{date} - Could not retrieve user token email!",
                DateTime.Now);

            return NoContent();
        }

        try
        {
            if (!MailManager.SendActivationEmail(createdUser.Id, userTokenDto.Value, createdUser.Email))
            {
                _logger.LogWarning($"{DateTime.Now} - Sending the activation email " +
                    $"to user with ID \"{createdUser.Id}\" failed!");
            }
        }
        catch (Exception e)
        {
            _logger.LogError($"{DateTime.Now} - An exception was thrown when trying to send " +
                $"the action email for user with ID \"{createdUser.Id}\" with token value " +
                $"\"{userTokenDto.Value}\" at address \"{createdUser.Email}\". " +
                $"Exception message : \"{e.Message}\"");
        }

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

        UserDto? user = _userService.GetUser(id);

        if (user is null)
        {
            return NotFound(id);
        }

        UserTokenDto? userToken = _userTokenService.GetUserToken(UserTokenTypes.EMAIL_CONFIRMATION_TOKEN, id);

        if (userToken is null)
        {
            return NotFound(token);
        }

        userToken.User = user;

        if (!_userTokenService.VerifyUserToken(userToken))
        {
            return BadRequest("This token has already been used");
        }

        return Ok();
    }
}
