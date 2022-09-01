using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.Helpers.Tools;
using API.DepotEice.UIL.Data;
using API.DepotEice.UIL.Interfaces;
using API.DepotEice.UIL.Managers;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using DevHopTools.Mappers;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace API.DepotEice.UIL.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;
    private readonly ITokenManager _tokenManager;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserTokenRepository _userTokenRepository;

    public AuthController(
        ILogger<AuthController> logger,
        IConfiguration configuration,
        ITokenManager tokenManager,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserTokenRepository userTokenRepository)
    {
        _logger = logger;
        _configuration = configuration;
        _tokenManager = tokenManager;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userTokenRepository = userTokenRepository;
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
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var entity = _userRepository.LogIn(form.Email, GenerateHash(form.Password));

            if (entity == null)
                return BadRequest("Email or Password are incorrect ! Please try again or contact the administration");

            // - récuperer l'utilisateur de la base de données,
            LoggedInUserModel? user = entity.Map<LoggedInUserModel>();
            user.Roles = _roleRepository.GetUserRoles(user.Id).Select(x => x.Map<RoleModel>());

            TokenModel token = new() { Token = _tokenManager.GenerateJWT(user) };

            return Ok(token);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
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
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            UserEntity? entity = _userRepository.GetUserByEmail(form.Email);

            if (entity != null)
                return BadRequest("There is already an account with this email! Please try with another mail or contact the administration.");

            entity = form.Map<UserEntity>();
            entity.PasswordHash = GenerateHash(form.Password);

            string userId = _userRepository.Create(entity);
            if (string.IsNullOrEmpty(userId) || string.IsNullOrWhiteSpace(userId))
                return BadRequest(nameof(userId));

            RoleModel guestRole = _roleRepository.GetByName(RolesData.GUEST_ROLE).Map<RoleModel>();
            if (guestRole == null)
                return BadRequest(nameof(guestRole));

            bool result = _roleRepository.AddUser(guestRole.Id, userId);
            if (!result)
                return BadRequest(nameof(result));

            entity = _userRepository.GetByKey(userId);

            string createdUserTokenID = _userTokenRepository.Create(new UserTokenEntity()
            {
                Type = UserTokenData.EMAIL_CONFIRMATION_TOKEN,
                ExpirationDateTime = DateTime.Now.AddDays(2),
                UserId = userId,
                UserSecurityStamp = entity.SecurityStamp
            });

            UserTokenEntity token = _userTokenRepository.GetByKey(createdUserTokenID);

            if (createdUserTokenID != null)
            {
                try
                {
                    if (!MailManager.SendActivationEmail(userId, token.Value, entity.NormalizedEmail))
                    {
                        _logger.LogWarning($"{DateTime.Now} - Sending the activation email to user with ID \"{userId}\" failed!");
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(
                        "{0} An exception was thrown when trying to send the action email for user with ID \"{1}\" with token value {2} at address {3}. Exception message : {4}",
                        DateTime.Now,
                        userId,
                        token.Value,
                        entity.NormalizedEmail,
                        e.Message);
                }
            }

            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost(nameof(Activate))]
    public IActionResult Activate(string id, string token)
    {
        try
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
                return BadRequest(nameof(id));

            if (string.IsNullOrEmpty(token) || string.IsNullOrWhiteSpace(token))
                return BadRequest(nameof(token));

            UserModel? user = _userRepository.GetByKey(id).Map<UserModel>();

            if (user == null)
                return NotFound("the user does not exist");

            // TODO - Erreure de récupération token
            UserTokenModel userToken = _userTokenRepository.GetByKey(token).Map<UserTokenModel>();

            //UserTokenDto? userToken = _userTokenService.GetUserToken(UserTokenTypes.EMAIL_CONFIRMATION_TOKEN, id);

            if (userToken is null)
                return NotFound(token);

            userToken.User = user;

            if (!_userTokenRepository.VerifyUserToken(userToken.Map<UserTokenEntity>()))
                return BadRequest("This token has already been used");

            return Ok("User have been activated with success");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    private string GetSalt()
    {
        return _configuration.GetValue<string>("AppSettings:Secret");
    }

    private string GenerateHash(string password)
    {
        return password.GenerateHMACSHA512(Encoding.UTF8.GetBytes(GetSalt()));
    }
}
