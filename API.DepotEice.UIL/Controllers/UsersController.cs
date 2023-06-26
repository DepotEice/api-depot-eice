using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.UIL.Data;
using API.DepotEice.UIL.Interfaces;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.DepotEice.UIL.Controllers;

/// <summary>
/// User controller
/// </summary>
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
    private readonly IUserManager _userManager;
    private readonly IFileManager _fileManager;
    private readonly IFileRepository _fileRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UsersController"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    /// <param name="userRepository">The user repository instance.</param>
    /// <param name="userTokenRepository">The user token repository instance.</param>
    /// <param name="roleRepository">The role repository instance.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <param name="userManager">The user manager instance.</param>
    /// <param name="fileManager">The file manager instance.</param>
    /// <param name="fileRepository">The file repository instance.</param>
    public UsersController(
        ILogger<UsersController> logger,
        IMapper mapper,
        IUserRepository userRepository,
        IUserTokenRepository userTokenRepository,
        IRoleRepository roleRepository,
        IConfiguration configuration,
        IUserManager userManager,
        IFileManager fileManager,
        IFileRepository fileRepository)
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

        if (userManager is null)
        {
            throw new ArgumentNullException(nameof(userManager));
        }

        if (fileManager is null)
        {
            throw new ArgumentNullException(nameof(fileManager));
        }

        if (fileRepository is null)
        {
            throw new ArgumentNullException(nameof(fileRepository));
        }

        _logger = logger;
        _mapper = mapper;
        _userRepository = userRepository;
        _userTokenRepository = userTokenRepository;
        _roleRepository = roleRepository;
        _configuration = configuration;
        _userManager = userManager;
        _fileManager = fileManager;
        _fileRepository = fileRepository;
    }

    /// <summary>
    /// Get information about the currently authenticated user.
    /// </summary>
    /// <returns>
    /// <see cref="StatusCodes.Status200OK"/> if the operation is successful.
    /// <see cref="StatusCodes.Status400BadRequest"/> if there is a bad request.
    /// <see cref="StatusCodes.Status401Unauthorized"/> if the caller is not authenticated.
    /// <see cref="StatusCodes.Status404NotFound"/> if the requested user does not exist.
    /// </returns>
    [HttpGet(nameof(Me))]
    public IActionResult Me()
    {
        try
        {
            string? userId = _userManager.GetCurrentUserId;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User must be authenticated!");
            }

            UserEntity? userFromRepo = _userRepository.GetByKey(userId);

            if (userFromRepo is null)
            {
                return NotFound($"There is no user with this user ID \"{userId}\"");
            }

            UserModel user = _mapper.Map<UserModel>(userFromRepo);

            return Ok(user);
        }
        catch (Exception e)
        {
            _logger.LogError(
                "{dt} - An exception was thrown during \"{fun}\":\"n{msg}\"\n{stack}",
                DateTime.Now,
                nameof(Me),
                e.Message,
                e.StackTrace
            );
#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest(
                "An error occurred while trying to get user's information (/Me), please contact the administrator"
            );
#endif
        }
    }

    /// <summary>
    /// Get all users.
    /// </summary>
    /// <returns><see cref="StatusCodes.Status200OK"/> if the operation is successful.</returns>
    [HttpGet()]
    public IActionResult Get()
    {
        try
        {
            var users = _userRepository.GetAll();
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
            _logger.LogError(
                $"{DateTime.Now} - An exception was thrown when trying to retrieve users from "
                    + $"repository and return the data.\n{e.Message}\n{e.StackTrace}"
            );
        }

        return Ok();
    }

    [HttpPost]
    public IActionResult Post([FromBody] UserForm form)
    {
        return Ok();
    }

    /// <summary>
    /// Updates a user with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the user to update.</param>
    /// <param name="form">The updated user information.</param>
    /// <returns>
    /// <see cref="StatusCodes.Status200OK"/> If the user was successfully updated.
    /// <see cref="StatusCodes.Status400BadRequest"/> If the ID or body is invalid.
    /// <see cref="StatusCodes.Status404NotFound"/> If the user with the specified ID does not exist.
    /// </returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] UserForm form)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest($"User ID required");
        }

        if (form is null)
        {
            return BadRequest($"The body cannot be null!");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            string? currentUserId = _userManager.GetCurrentUserId;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized("You must be authenticated to perform this action");
            }

            if (!currentUserId.Equals(id))
            {
                if (!_userManager.IsInRole(RolesData.DIRECTION_ROLE))
                {
                    return Unauthorized("You are not authorized to modify other user's information");
                }
            }

            UserEntity? userFromRepo = _userRepository.GetByKey(id);

            if (userFromRepo is null)
            {
                return NotFound($"The requested user does not exist");
            }

            _mapper.Map(form, userFromRepo);

            if (form.ProfilePicture is not null)
            {
                if (!await _fileManager.UploadObjectAsync(form.ProfilePicture, $"profile-picture-{id}"))
                {
                    _logger.LogError("{dt} - Uploading the file for user \"{id}\" in AWS failed",
                        DateTime.Now,
                        id);

                    return BadRequest(
                    "An error occurred while trying to upload the profile picture, please contact the administrator"
                    );
                }

                FileEntity fileEntity = new FileEntity()
                {
                    Key = $"profile-picture-{id}",
                    Type = form.ProfilePicture.ContentType,
                    Size = form.ProfilePicture.Length
                };

                int createdFileId = _fileRepository.Create(fileEntity);

                if (createdFileId == 0)
                {
                    _logger.LogError("{dt} - The file for user \"{id}\" was not saved in the database",
                        DateTime.Now,
                        id);

                    return BadRequest(
                        "An error occurred while trying to save the profile picture, please contact the administrator"
                    );
                }

                userFromRepo.ProfilePictureId = createdFileId;
            }

            if (!_userRepository.Update(id, userFromRepo))
            {
                _logger.LogError("{dt} - Updating user \"{id}\" failed",
                    DateTime.Now,
                    id);

                return BadRequest(
                    "An error occurred while trying to update user's information, please contact the administrator");
            }

            userFromRepo = _userRepository.GetByKey(id);

            UserModel user = _mapper.Map<UserModel>(userFromRepo);

            return Ok(user);
        }
        catch (Exception e)
        {
            _logger.LogError("{dt} - An exception was thrown during \"{fun}\":\"n{msg}\"\n{stack}",
                DateTime.Now,
                nameof(Put),
                e.Message,
                e.StackTrace
            );

#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest(
                "An error occurred while trying to update user's information, please contact the administrator"
            );
#endif
        }
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
    /// <returns></returns>
    [HttpPost(nameof(UpdatePassword))]
    public IActionResult UpdatePassword([FromBody] PasswordForm passwordForm)
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
                return Unauthorized(
                    "The password you are trying to reset is not associated to your account!"
                );
            }

            bool result = _userRepository.UpdatePassword(
                passwordForm.UserId,
                passwordForm.Password,
                GetSalt()
            );

            if (!result)
            {
                return BadRequest("Password update failed!");
            }

            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(
                $"{DateTime.Now} - An exception was thrown during \"{nameof(UpdatePassword)}\" : "
                    + $"\"{e.Message}\"\n\"{e.Message}\""
            );

#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest(
                "An error occurred while trying to update the password, please contact the administrator"
            );
#endif
        }
    }

    private string GetSalt()
    {
#if DEBUG
        return _configuration.GetValue<string>("AppSettings:Secret");
#else
        return Environment.GetEnvironmentVariable("PASSWORD_SALT")
            ?? throw new NullReferenceException(
                $"{DateTime.Now} - There is no environment variable named " + $"\"PASSWORD_SALT\""
            );
#endif
    }
}
