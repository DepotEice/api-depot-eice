using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.Helpers.Tools;
using API.DepotEice.UIL.AuthorizationAttributes;
using API.DepotEice.UIL.Data;
using API.DepotEice.UIL.Interfaces;
using API.DepotEice.UIL.Managers;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using AutoMapper;
using DevHopTools.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS.Core;
using Newtonsoft.Json.Linq;
using System.CodeDom.Compiler;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using static API.DepotEice.UIL.Data.RolesData;

namespace API.DepotEice.UIL.Controllers;

/// <summary>
/// Take charge of all the endpoints related to the authentication like Login, Register and so on
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly ITokenManager _tokenManager;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserTokenRepository _userTokenRepository;
    private readonly IUserManager _userManager;

    /// <summary>
    /// Instanciate the AuthController. Each parameter being injected
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="mapper"></param>
    /// <param name="configuration"></param>
    /// <param name="tokenManager"></param>
    /// <param name="userRepository"></param>
    /// <param name="roleRepository"></param>
    /// <param name="userTokenRepository"></param>
    /// <param name="userManager"></param>
    public AuthController(ILogger<AuthController> logger, IMapper mapper, IConfiguration configuration,
        ITokenManager tokenManager, IUserRepository userRepository, IRoleRepository roleRepository,
        IUserTokenRepository userTokenRepository, IUserManager userManager)
    {
        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        if (mapper is null)
        {
            throw new ArgumentNullException(nameof(mapper));
        }

        if (configuration is null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        if (tokenManager is null)
        {
            throw new ArgumentNullException(nameof(tokenManager));
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

        if (userManager is null)
        {
            throw new ArgumentNullException(nameof(userManager));
        }

        _logger = logger;
        _mapper = mapper;
        _configuration = configuration;
        _tokenManager = tokenManager;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userTokenRepository = userTokenRepository;
        _mapper = mapper;
        _userManager = userManager;
    }

    /// <summary>
    /// If user's credentials are correct, sign in the user and generate and return a JWT Token
    /// </summary>
    /// <param name="loginForm"></param>
    /// <returns>
    /// <see cref="StatusCodes.Status200OK"/> If the user credentials are correct and a JWT 
    /// tokenValue was generated
    /// <para />
    /// <see cref="StatusCodes.Status400BadRequest"/> If an error occured during authentication
    /// </returns>
    [HttpPost(nameof(Login))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Login([FromBody] LoginForm loginForm)
    {
        if (loginForm is null)
        {
            return BadRequest("The body content cannot be null!");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            UserEntity? userFromRepo = _userRepository.LogIn(loginForm.Email, loginForm.Password, GetSalt());

            if (userFromRepo is null)
            {
                return BadRequest("Email or Password are incorrect ! Please try again or contact the " +
                    "administration");
            }

            if (userFromRepo.DeletedAt is not null)
            {
                return BadRequest("This account has been deleted, please contact the administration");
            }

            LoggedInUserModel? user = userFromRepo.Map<LoggedInUserModel>();

            user.Roles = _roleRepository.GetUserRoles(user.Id).Select(x => x.Map<RoleModel>());

            TokenModel token = new()
            {
                Token = _tokenManager.GenerateJWT(user)
            };

            return Ok(token);
        }
        catch (Exception e)
        {
            _logger.LogError($"{DateTime.Now} - An exception was thrown during \"{nameof(Login)}\" :\n" +
                $"\"{e.Message}\"\n\"{e.StackTrace}\"");

#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest("An error occurred while trying to login, please contact the administrator");
#endif
        }
    }

    /// <summary>
    /// Create a new inactive user, assign him to a GUEST role and send an activation email
    /// </summary>
    /// <param name="registerForm">
    /// The user registerForm for registration in json format
    /// </param>
    /// <returns>
    /// <see cref="StatusCodes.Status200OK"/> If the user registration was successful.
    /// <para/>
    /// <see cref="StatusCodes.Status400BadRequest"/>
    /// <para/>
    /// </returns>
    [HttpPost(nameof(Register))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterForm registerForm)
    {
        if (registerForm is null)
        {
            return BadRequest($"The body content cannot be null!");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            UserEntity? userEntity = _userRepository.GetUserByEmail(registerForm.Email);

            if (userEntity is not null)
            {
                return BadRequest("There is already an account with this email! Please try with another mail or " +
                    "contact the administration.");
            }

            userEntity = _mapper.Map<UserEntity>(registerForm);

            string? userId = _userRepository.Create(userEntity, registerForm.Password, GetSalt());

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User creation failed");
            }

            RoleEntity? guestRoleFromRepo = _roleRepository.GetByName(RolesData.GUEST_ROLE);

            if (guestRoleFromRepo is null)
            {
                string roleId = _roleRepository.Create(new RoleEntity()
                {
                    Name = RolesData.GUEST_ROLE
                });

                if (string.IsNullOrEmpty(roleId))
                {
                    return BadRequest();
                }

                guestRoleFromRepo = _roleRepository.GetByName(RolesData.GUEST_ROLE);

                if (guestRoleFromRepo is null)
                {
                    return BadRequest(guestRoleFromRepo);
                }
            }

            RoleModel guestRole = _mapper.Map<RoleModel>(guestRoleFromRepo);

            bool result = _roleRepository.AddUser(guestRole.Id, userId);

            if (!result)
            {
                return BadRequest($"An error occurred while trying to add the user to the role \"{RolesData.GUEST_ROLE}\"");
            }

            userEntity = _userRepository.GetByKey(userId);

            if (userEntity is null)
            {
                return NotFound("The newly created user couldn't be found!");
            }

            string createdUserTokenID = _userTokenRepository.Create(new UserTokenEntity()
            {
                Type = TokenTypesData.EMAIL_CONFIRMATION_TOKEN,
                ExpirationDate = DateTime.Now.AddDays(2),
                UserId = userId,
                UserSecurityStamp = userEntity.SecurityStamp
            });

            UserTokenEntity? token = _userTokenRepository.GetByKey(createdUserTokenID);

            if (token is null)
            {
                return BadRequest("Token couldn't be created");
            }

            if (createdUserTokenID is not null)
            {
                try
                {
                    if (!await MailManager.SendActivationEmailAsync(token.Id, token.Value, userEntity.NormalizedEmail))
                    {
                        _logger.LogWarning("{date} - Sending the activation email to user with ID \"{userId}\" " +
                            "failed!", DateTime.Now, userId);

                        return BadRequest("The activation email couldn't be sent, please contact the administror");
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError($"{DateTime.Now} An exception was thrown when trying to send the action email " +
                        $"for user with ID \"{userId}\" with token value {token.Value} at address {userEntity.NormalizedEmail}.\n" +
                        $"\"{e.Message}\"\"n\"{e.StackTrace}\"");

#if DEBUG
                    return BadRequest(e.Message);
#else
                    return BadRequest("An error occurred while trying to send the activation email, please contact the administrator");
#endif
                }
            }

            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError($"{DateTime.Now} - An exception was thrown during \"{nameof(Register)}\" :\n" +
                $"\"{e.Message}\"\n\"{e.StackTrace}\"");
#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest("An error occurred while trying to register, please contact the administrator");
#endif
        }
    }

    /// <summary>
    /// Reset user's password by providing the tokenValue received by mail
    /// </summary>
    /// <param name="passwordForm">The Password registerForm</param>
    /// <param name="token">The tokenValue provided by mail</param>
    /// <returns>
    /// 204 No content if the operation was successful
    /// </returns>
    [HttpPost(nameof(ResetPassword))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult ResetPassword([FromBody] PasswordForm passwordForm, string token)
    {
        if (passwordForm is null)
        {
            return BadRequest("The body content cannot be null!");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (string.IsNullOrEmpty(token) || string.IsNullOrWhiteSpace(token))
        {
            return BadRequest("The token is required");
        }

        try
        {
            UserTokenEntity? userTokenFromRepo = _userTokenRepository
                .GetUserTokens(passwordForm.UserId)
                .FirstOrDefault(ut => ut.Value.Equals(token) && ut.ExpirationDate > DateTime.Now);

            if (userTokenFromRepo is null)
            {
                return NotFound("Token doesn't exist");
            }

            if (!_userTokenRepository.ApproveToken(userTokenFromRepo))
            {
                return BadRequest("Token is invalid or expired");
            }

            if (!passwordForm.UserId.Equals(userTokenFromRepo.UserId))
            {
                return Unauthorized();
            }

            bool result = _userRepository.UpdatePassword(passwordForm.UserId, passwordForm.Password, GetSalt());

            if (!result)
            {
                return BadRequest("Password update failed");
            }

            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(
                "{date} - An exception was thrown during {fn} :\n\"{eMsg}\"\n\"{eStr}\"",
                DateTime.Now,
                nameof(ResetPassword),
                e.Message,
                e.StackTrace
            );

#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest("An error occurred while trying to reset the password, please contact the administrator");
#endif
        }
    }

    /// <summary>
    /// Request a new password. Called when the user forgot his password. Only takes the email address in parameters
    /// </summary>
    /// <param name="email">The email address of the user</param>
    /// <returns>
    /// <see cref="StatusCodes.Status200OK"/> If the operation was successful
    /// <see cref="StatusCodes.Status404NotFound"/> If there is no user having the given email or if the newly created
    /// tokenValue couldn't be found
    /// <see cref="StatusCodes.Status400BadRequest"/>
    /// </returns>
    [HttpPost(nameof(RequestPassword))]
    public async Task<IActionResult> RequestPassword(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return BadRequest("The body content cannot be null or empty");
        }

        try
        {
            UserEntity? userFromRepo = _userRepository.GetUserByEmail(email);

            if (userFromRepo is null)
            {
#if DEBUG
                return NotFound($"There is no user with this email address : \"{email}\"");
#else
                return Ok();
#endif
            }

            string createdUserTokenID = _userTokenRepository.Create(new UserTokenEntity()
            {
                Type = TokenTypesData.PASSWORD_FORGET,
                ExpirationDate = DateTime.Now.AddHours(2),
                UserId = userFromRepo.Id,
                UserSecurityStamp = userFromRepo.SecurityStamp
            });

            if (string.IsNullOrEmpty(createdUserTokenID))
            {
                return BadRequest("An error occured during token creation");
            }

            UserTokenEntity? tokenFromRepo = _userTokenRepository.GetByKey(createdUserTokenID);

            if (tokenFromRepo is null)
            {
                return NotFound("The newly created token couldn't be found");
            }

            bool result = await MailManager.SendPasswordRequestEmailAsync(userFromRepo.Id, tokenFromRepo.Value, email);

            if (!result)
            {
                return BadRequest("The mail couldn't be sent!, please contact the administrator");
            }

            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError($"{DateTime.Now} - An exception was thrown during \"{nameof(RequestPassword)}\" :\n" +
                $"\"{e.Message}\"\n\"{e.StackTrace}\"");
#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest("An error occurred while trying to request a new password, please contact the administrator");
#endif
        }
    }

    // TODO : Rename the method to 'VerifyEmail' ?
    // TODO : Move the Guest role assignation to this method instead of Register
    /// <summary>
    /// Activate the User's account by verifying the passed token in parameter with the existing token in the database
    /// </summary>
    /// <param name="tokenId">The ID of the token to verify</param>
    /// <param name="tokenValue">The value of the token received by mail</param>
    /// <returns>
    /// <see cref="StatusCodes.Status200OK"/> If the activation was successful
    /// <see cref="StatusCodes.Status404NotFound"/> If the requested token doesn't exist or if the user requesting it
    /// doesn't exist either
    /// <see cref="StatusCodes.Status400BadRequest"/>
    /// </returns>
    [HttpPost(nameof(Activate))]
    public IActionResult Activate(string tokenId, string tokenValue)
    {
        if (string.IsNullOrEmpty(tokenId) || string.IsNullOrWhiteSpace(tokenId))
        {
            return BadRequest("The token ID cannot be null or empty");
        }

        if (string.IsNullOrEmpty(tokenValue) || string.IsNullOrWhiteSpace(tokenValue))
        {
            return BadRequest("The token value cannot be null or empty");
        }

        try
        {
            UserTokenEntity? userToken = _userTokenRepository.GetByKey(tokenId);

            if (userToken is null)
            {
                return NotFound("The token does not exist!");
            }

            UserEntity? user = _userRepository.GetByKey(userToken.UserId);

            if (user is null)
            {
                return NotFound("the user does not exist");
            }

            userToken.UserId = user.Id;
            userToken.UserSecurityStamp = user.SecurityStamp;

            if (!_userTokenRepository.VerifyUserToken(userToken))
            {
                return BadRequest("This token has already been used");
            }

            if (!_userRepository.UpdateActivationStatus(user.Id))
            {
                return BadRequest("User activation failed!");
            }

            return Ok("User has been successfully activated");
        }
        catch (Exception e)
        {
            _logger.LogError(
                "{date} - An exception was thrown during \"{fn}\" :\n\"{eMsg}\"\n\"{eStr}\"",
                DateTime.Now,
                nameof(Activate),
                e.Message,
                e.StackTrace
            );
#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest("An error occurred while trying to active the user, please contact the administrator");
#endif
        }
    }

    /// <summary>
    /// Validate the JWT Token and return user's roles and permissions
    /// </summary>
    /// <param name="jwtToken">The User's JWT Token</param>
    /// <returns>
    /// <see cref="StatusCodes.Status200OK"/> with the boolean result of the operation
    /// <see cref="StatusCodes.Status400BadRequest"/>
    /// <see cref="StatusCodes.Status401Unauthorized"/> If the current user is null or empty
    /// </returns>
    [HttpGet($"{nameof(Authorize)}/{{jwtToken}}")]
    [HasRoleAuthorize(RolesEnum.GUEST)]
    public IActionResult Authorize(string jwtToken)
    {
        if (string.IsNullOrEmpty(jwtToken))
        {
            return BadRequest("The token in parameter cannot null or empty");
        }

        try
        {
            string? currentUserId = _userManager.GetCurrentUserId;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized();
            }

            bool result = _tokenManager.ValidateJwtToken(currentUserId, jwtToken);

            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError($"{DateTime.Now} - An exception was thrown during \"{nameof(Authorize)}\" :\n" +
                $"\"{e.Message}\"\n\"{e.StackTrace}\"");
#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest("An error occurred while trying to authorize user, please contact the administrator");
#endif

        }
    }

    private string GetSalt()
    {
#if DEBUG
        return _configuration.GetValue<string>("AppSettings:Salt");
#else
        return Environment.GetEnvironmentVariable("PASSWORD_SALT") ??
            throw new NullReferenceException($"{DateTime.Now} - There is no environment variable named " +
                $"\"PASSWORD_SALT\"");
#endif
    }
}
