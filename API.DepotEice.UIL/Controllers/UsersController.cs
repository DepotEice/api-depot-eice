using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.UIL.Data;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.DepotEice.UIL.Controllers;

// TODO : Implements methods

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly IUserTokenRepository _userTokenRepository;
    private readonly IRoleRepository _roleRepository;

    public UsersController(ILogger<UsersController> logger, IMapper mapper, IUserRepository userRepository,
        IUserTokenRepository userTokenRepository, IRoleRepository roleRepository, IConfiguration configuration)
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

        if (roleRepository is null)
        {
            throw new ArgumentNullException(nameof(roleRepository));
        }

        if (configuration is null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        _logger = logger;
        _mapper = mapper;
        _userRepository = userRepository;
        _userTokenRepository = userTokenRepository;
        _roleRepository = roleRepository;
        _configuration = configuration;
    }

    [HttpGet()]
    public IActionResult Get()
    {
        try
        {
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("teachers")]
    public IActionResult GetTeachers()
    {
        try
        {
            var users = _userRepository.GetUsersByRole(RolesData.TEACHER_ROLE);

            return Ok(users);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{id}")]
    public IActionResult Get(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest($"Provide the correct ID");
        }

        try
        {
            var userFromRepo = _userRepository.GetByKey(id);

            if (userFromRepo is null)
            {
                return NotFound($"The requested user does not exist");
            }

            UserModel user = _mapper.Map<UserModel>(userFromRepo);

            return Ok(user);
        }
        catch (Exception e)
        {
            _logger.LogError($"{DateTime.Now} - An exception was thrown when trying to retrieve users from " +
                $"repository and return the data.\n{e.Message}\n{e.StackTrace}");
        }

        return Ok();
    }

    [HttpPost]
    public IActionResult Post([FromBody] UserForm form)
    {
        return Ok();
    }

    [HttpPut("{id}")]
    public IActionResult Put(string id, [FromBody] UserForm form)
    {
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        return Ok();
    }

    [HttpGet("{uId}/Appointments")]
    public IActionResult GetAppointments(string uId)
    {
        return Ok();
    }

    [HttpPost("{uId}/Appointments")]
    public IActionResult PostAppointment(string uId, [FromBody] AppointmentForm form)
    {
        return Ok();
    }

    /// <summary>
    /// Update user's password
    /// </summary>
    /// <param name="passwordForm"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [HttpPost(nameof(UpdatePassword))]
    public IActionResult UpdatePassword([FromBody] PasswordForm passwordForm, string? token = null)
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
            string? userId = User.Claims.SingleOrDefault(c => c.Type.Equals(ClaimTypes.Sid))?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated!");
            }

            if (!userId.Equals(passwordForm.UserId))
            {
                return Unauthorized("The password you are trying to reset is not associated to your account!");
            }

            bool result = _userRepository.UpdatePassword(passwordForm.UserId, passwordForm.Password, GetSalt());

            if (!result)
            {
                return BadRequest("Password update failed!");
            }

            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError($"{DateTime.Now} - An exception was thrown during {nameof(UpdatePassword)} : " +
                $"\"{e.Message}\"\n\"{e.Message}\"");

#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest("An error occurred while trying to update the password, please contact the administrator");
#endif
        }
    }

    private string GetSalt()
    {
#if DEBUG
        return _configuration.GetValue<string>("AppSettings:Secret");
#else
        return Environment.GetEnvironmentVariable("PASSWORD_SALT") ??
            throw new NullReferenceException($"{DateTime.Now} - There is no environment variable named " +
                $"\"PASSWORD_SALT\"");
#endif
    }
}
