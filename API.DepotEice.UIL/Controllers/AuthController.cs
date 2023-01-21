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
using Newtonsoft.Json.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
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
    [HttpPost(nameof(Login))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Login([FromBody] LoginForm form)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            UserEntity? userFromRepo = _userRepository.LogIn(form.Email, form.Password, GetSalt());

            if (userFromRepo is null)
            {
                return BadRequest("Email or Password are incorrect ! Please try again or contact the " +
                    "administration");
            }

            // TODO : Adapt the following code because it is really strange

            LoggedInUserModel? user = userFromRepo.Map<LoggedInUserModel>();
            user.Roles = _roleRepository.GetUserRoles(user.Id).Select(x => x.Map<RoleModel>());

            TokenModel token = new() { Token = _tokenManager.GenerateJWT(user) };

            return Ok(token);
        }
        catch (Exception e)
        {
            _logger.LogError($"{DateTime.Now} - An exception was thrown during \"{nameof(Login)}\" : " +
                $"\"{e.Message}\"\n\"{e.StackTrace}\"");

#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest("An error occurred while trying to update the password, please contact the administrator");
#endif
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
    public async Task<IActionResult> SignUp([FromBody] RegisterForm form)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            UserEntity? userEntity = _userRepository.GetUserByEmail(form.Email);

            if (userEntity is not null)
                return BadRequest("There is already an account with this email! Please try with another mail or contact the administration.");

            userEntity = form.Map<UserEntity>();

            // TODO : Remove the hardcoded hash
            string userId = _userRepository.Create(userEntity, form.Password, GetSalt());

            if (string.IsNullOrEmpty(userId) || string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest(nameof(userId));
            }

            RoleModel? guestRole = _roleRepository.GetByName(RolesData.GUEST_ROLE).Map<RoleModel>();

            if (guestRole is null)
            {
                string roleId = _roleRepository.Create(new RoleEntity()
                {
                    Name = RolesData.GUEST_ROLE
                });

                if (string.IsNullOrEmpty(roleId))
                {
                    return BadRequest();
                }

                guestRole = _roleRepository.GetByName(RolesData.GUEST_ROLE).Map<RoleModel>();

                if (guestRole is null)
                {
                    return BadRequest(guestRole);
                }
            }

            bool result = _roleRepository.AddUser(guestRole.Id, userId);

            if (!result)
            {
                return BadRequest(nameof(result));
            }

            userEntity = _userRepository.GetByKey(userId);

            if (userEntity is null)
            {
                return NotFound("User couldn't be created!");
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
                    //if (!MailManager.SendActivationEmail(token.Id, token.Value, userEntity.NormalizedEmail))
                    if (!await MailManager.SendActivationEmailAsync(token.Id, token.Value, userEntity.NormalizedEmail))
                    {
                        _logger.LogWarning("{date} - Sending the activation email to user with ID \"{userId}\" " +
                            "failed!", DateTime.Now, userId);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError("{date} An exception was thrown when trying to send the action email for user " +
                        "with ID \"{userId}\" with token value {tokenValue} at address {normalizedEmail}. Exception " +
                        "message : {message}",
                                     DateTime.Now,
                                     userId,
                                     token.Value,
                                     userEntity.NormalizedEmail,
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

    /// <summary>
    /// Reset user's password by providing the token received by mail
    /// </summary>
    /// <param name="passwordForm">The Password form</param>
    /// <param name="token">The token provided by mail</param>
    /// <returns></returns>
    [HttpPost(nameof(ResetPassword))]
    public IActionResult ResetPassword([FromBody] PasswordForm passwordForm, string token)
    {
        if (passwordForm is null)
        {
            return BadRequest("The body cannot be null!");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
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
            _logger.LogError($"{DateTime.Now} - An exception was thrown during {nameof(ResetPassword)} : " +
                $"\"{e.Message}\"\n\"{e.Message}\"");

#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest("An error occurred while trying to update the password, please contact the administrator");
#endif
        }
    }

    [HttpGet(nameof(RequestNewPassword))]
    public async Task<IActionResult> RequestNewPassword(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return BadRequest();
        }

        try
        {
            UserEntity? userFromRepo = _userRepository.GetUserByEmail(email);

            if (userFromRepo is null)
            {
                return BadRequest();
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
                return NotFound("Created token couldn't be retrieved");
            }

            //bool result = MailManager.SendPasswordRequestEmail(userFromRepo.Id, tokenFromRepo.Value, email);
            bool result = await MailManager.SendPasswordRequestEmailAsync(userFromRepo.Id, tokenFromRepo.Value, email);

            if (!result)
            {
                return BadRequest("The mail couldn't be sent!");
            }

            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [HttpPost(nameof(Activate))]
    public IActionResult Activate(string id, string token)
    {
        try
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(nameof(id));
            }

            if (string.IsNullOrEmpty(token) || string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(nameof(token));
            }

            UserTokenEntity? userToken = _userTokenRepository.GetByKey(id);

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

            if (!_userRepository.ActivateDeactivateUser(user.Id))
            {
                return BadRequest("User activation failed!");
            }

            return Ok("User has been successfully activated");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
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
