using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.UIL.AuthorizationAttributes;
using API.DepotEice.UIL.Data;
using API.DepotEice.UIL.Interfaces;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static API.DepotEice.UIL.Data.RolesData;

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
    private readonly IAddressRepository _addressRepository;

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
        IFileRepository fileRepository
,
        IAddressRepository addressRepository)
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

        if (addressRepository is null)
        {
            throw new ArgumentNullException(nameof(addressRepository));
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
        _addressRepository = addressRepository;
    }

    /// <summary>
    /// Get all the students
    /// </summary>
    /// <returns>
    /// List of all the students
    /// </returns>
    [HttpGet("Students")]
    [HasRoleAuthorize(RolesEnum.DIRECTION)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserModel>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GetStudents()
    {
        try
        {
            IEnumerable<UserEntity> usersFromRepo = _userRepository
                .GetUsersByRole(STUDENT_ROLE)
                .Where(u => u.DeletedAt is null);

            IEnumerable<UserModel> users = _mapper.Map<IEnumerable<UserModel>>(usersFromRepo);

            return Ok(users);
        }
        catch (Exception e)
        {
            _logger.LogError(
                "{date} - An exception was thrown during \"{fnName}\":\n{e.Message}\"\n\"{e.StackTrace}\"",
                DateTime.Now,
                nameof(GetStudents),
                e.Message,
                e.StackTrace
            );
#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest("An error occurred while trying to get students, please contact the administrator");
#endif
        }
    }

    /// <summary>
    /// Update the profile picture of the user
    /// </summary>
    /// <param name="files">The form file</param>
    /// <param name="id">The id of the user</param>
    /// <returns></returns>
    [HttpPost("UpdateProfilePicture")]
    public async Task<IActionResult> UpdateProfilePictureAsync([FromForm] IEnumerable<IFormFile> files, [FromQuery] string? id)
    {
        try
        {
            // Check if the list of files contains more than one file
            if (Request.Form.Files.Count != 1)
            {
                return BadRequest("You can only upload one file at a time");
            }

            // Set the user ID
            string? userId = string.Empty;

            // Check if the user ID is provided
            if (!string.IsNullOrEmpty(id))
            {
                // Check if the user is in the direction role
                if (!_userManager.IsInRole(RolesData.DIRECTION_ROLE))
                {
                    return Unauthorized("The user is not authorized to create a profile picture for another user");
                }
            }
            // If the user ID is not provided, check if the user is authenticated
            else
            {
                userId = _userManager.GetCurrentUserId;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("The user is not authenticated");
                }
            }

            // Get the user from the database
            UserEntity? userFromRepo = _userRepository.GetByKey(userId);

            // Check if the user exists, if not return a 404 error
            if (userFromRepo is null)
            {
                return NotFound($"There is no user with this user ID \"{id}\"");
            }

            // Check if the user has a profile picture ID
            if (userFromRepo.ProfilePictureId.HasValue && userFromRepo.ProfilePictureId.Value >= 0)
            {
                // Get the existing file from the database
                FileEntity? existingFileFromRepo = _fileRepository.GetByKey(userFromRepo.ProfilePictureId.Value);

                // Check if the file exists
                if (existingFileFromRepo is not null)
                {
                    // Delete the file from the file manager (AWS)
                    if (!await _fileManager.DeleteObjectAsync(existingFileFromRepo.Key))
                    {
                        return BadRequest("The deletion of the existing file failed");
                    }

                    // Delete the file from the database
                    if (!_fileRepository.Delete(existingFileFromRepo.Id))
                    {
                        return BadRequest($"The deletion of the existing file failed in the database for the " +
                            $"file with the ID \"{existingFileFromRepo.Id}\"");
                    }
                }
                // If the file does not exist in the database but the user has a profile picture ID, log a warning
                else
                {
                    _logger.LogWarning("The user has a profile picture ID but the file does not exist in the database");
                }
            }

            // Get the file from the list of files
            IFormFile? file = Request.Form.Files.SingleOrDefault();

            // Check if the file is null
            if (file is null)
            {
                return BadRequest("The file is not defined");
            }

            // Check if the file is empty
            if (file.Length == 0)
            {
                return BadRequest("The file is empty");
            }

            // Check if the file is too big
            if (file.Length > 2097152)
            {
                return BadRequest("The file is too big");
            }

            if (!file.ContentType.Contains("image/"))
            {
                return BadRequest("The file is not an image");
            }

            string? fileExtension = file.ContentType.Split("/").LastOrDefault();

            if (string.IsNullOrEmpty(fileExtension))
            {
                return BadRequest("The file has no extension");
            }

            // Check if the file is a valid image
            if (!fileExtension.Equals("png", StringComparison.OrdinalIgnoreCase) &&
                !fileExtension.Equals("jpg", StringComparison.OrdinalIgnoreCase) &&
                !fileExtension.Equals("jpeg", StringComparison.OrdinalIgnoreCase) &&
                !fileExtension.Equals("gif", StringComparison.OrdinalIgnoreCase) &&
                !fileExtension.Equals("svg", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("The file is not a valid image");
            }

            // Create the file name
            string? fileName = $"{userId}-profile-picture-{DateTime.Now.ToString("yyyyMMdd-HHmmss")}.{fileExtension}";

            // Upload the file to the file manager (AWS)
            if (!await _fileManager.UploadObjectAsync(file, fileName))
            {
                return BadRequest("The upload of the file failed");
            }

            FileModel? fileFromAWS = await _fileManager.GetObjectAsync(fileName);

            // Check if the file was uploaded to the file manager (AWS), if not return a 400 error
            if (fileFromAWS is null)
            {
                return BadRequest("An error occurred while trying to save the profile picture");
            }

            // Create the file entity
            FileEntity? fileEntity = new()
            {
                Key = fileName,
                Size = file.Length,
                Type = file.ContentType,
                CreatedAt = DateTime.Now,
            };

            // Save the file to the database
            int newFileId = _fileRepository.Create(fileEntity);

            // Check if the file was saved to the database, if not return a 400 error
            if (newFileId <= 0)
            {
                return BadRequest("An error occurred while trying to save the profile picture");
            }

            // Get the file from the database
            FileEntity? fileFromRepo = _fileRepository.GetByKey(newFileId);

            // Check if the file exists, if not return a 404 error
            if (fileFromRepo is null)
            {
                return NotFound("An error occurred while trying to save the profile picture");
            }

            // Update the user with the new profile picture ID
            userFromRepo.ProfilePictureId = newFileId;

            // Save the user to the database, if not return a 400 error
            if (!_userRepository.Update(userId, userFromRepo))
            {
                return BadRequest("The file could not be saved to the user");
            }

            // Return 200 OK with the file
            return File(fileFromAWS.Content, fileFromAWS.ContentType);
        }
        catch (Exception e)
        {
            _logger.LogError($"{DateTime.Now} - An exception was thrown during \"{nameof(UpdateProfilePictureAsync)}\" :\n" +
               $"\"{e.Message}\"\n\"{e.StackTrace}\"");

#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest("An error occurred while trying to change the profile picture, please contact the administrator");
#endif
        }
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
    [Authorize]
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
            _logger.LogError($"{DateTime.Now} - An exception was thrown during \"{nameof(Me)}\" :\n" +
               $"\"{e.Message}\"\n\"{e.StackTrace}\"");
#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest("An error occurred while trying to get user's info, please contact the administrator");
#endif
        }
    }

    /// <summary>
    /// Get the profile picture of the user with the given ID. If no ID is given, the profile picture of the currently 
    /// authenticated user is returned.
    /// </summary>
    /// <param name="id">The id of the user</param>
    /// <returns>
    /// The file content
    /// </returns>
    [HttpGet("ProfilePicture")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileContentResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfilePicture([FromQuery] string? id)
    {
        try
        {
            string? userId = string.IsNullOrEmpty(id) ? _userManager.GetCurrentUserId : id;

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User must be authenticated!");
            }

            UserEntity? userFromRepo = _userRepository.GetByKey(userId);

            if (userFromRepo is null)
            {
                return NotFound($"There is no user with this user ID \"{userId}\"");
            }

            int? userProfilePictureId = userFromRepo.ProfilePictureId;

            FileModel? fileFromAws = null;

            if (!userProfilePictureId.HasValue)
            {
                fileFromAws = await _fileManager.GetObjectAsync(Utils.DefaultProfilePicture);
            }
            else
            {
                FileEntity? fileFromRepo = _fileRepository.GetByKey(userProfilePictureId.Value);

                if (fileFromRepo is null)
                {
                    return NotFound($"There is no file with this file ID \"{userProfilePictureId.Value}\"");
                }

                fileFromAws = await _fileManager.GetObjectAsync(fileFromRepo.Key);
            }

            if (fileFromAws is null)
            {
                return NotFound($"There is no file for \"{Utils.DefaultProfilePicture}\"");
            }

            return File(fileFromAws.Content, fileFromAws.ContentType, fileFromAws.FileName);
        }
        catch (Exception e)
        {
            _logger.LogError(
                "{date} - An exception was thrown during \"{fn}\" :\n\"{eMsg}\"\n\"{eStr}\"",
                DateTime.Now,
                nameof(GetProfilePicture),
                e.Message,
                e.StackTrace
            );

#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest("An error occurred while trying to get user's profile picture, please contact the administrator");
#endif
        }
    }

    /// <summary>
    /// Get all users.
    /// </summary>
    /// <returns>All the users in the app</returns>
    [HttpGet]
    [HasRoleAuthorize(RolesEnum.DIRECTION)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserModel>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Get()
    {
        try
        {
            IEnumerable<UserEntity> users = _userRepository.GetAll();

            IEnumerable<UserModel> usersToReturn = _mapper.Map<IEnumerable<UserModel>>(users);

            return Ok(usersToReturn);
        }
        catch (Exception e)
        {
            _logger.LogError(
                "{date} - An exception was thrown during \"{fn}\" :\n{eMsg}\n{eStr}",
                DateTime.Now,
                nameof(Get),
                e.Message,
                e.StackTrace
            );

#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest("An error occurred while trying to get users, please contact the administrator");
#endif
        }
    }

    /// <summary>
    /// Get all the available users from the database without the current user and the deleted users
    /// </summary>
    /// <returns>
    /// List of all the users.
    /// </returns>
    [HttpGet("available")]
    [HasRoleAuthorize(RolesEnum.GUEST)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserModel>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetAvailableUsers()
    {
        try
        {
            string? currentUserId = _userManager.GetCurrentUserId;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized("You must be authenticated to perform this action!");
            }

            IEnumerable<UserEntity> usersFromRepo = _userRepository
                .GetAll()
                .Where(u => !u.Id.Equals(currentUserId) && u.DeletedAt is null);

            IEnumerable<UserModel> usersToReturn = _mapper.Map<IEnumerable<UserModel>>(usersFromRepo);

            return Ok(usersToReturn);
        }
        catch (Exception e)
        {
            _logger.LogError(
                "{date} - An exception was thrown during \"{fn}\" :\n{eMsg}\"\n\"{eStr}\"",
                DateTime.Now,
                nameof(GetAvailableUsers),
                e.Message,
                e.StackTrace
            );

#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest("An error occurred while trying to get available users, please contact the administrator");
#endif
        }
    }

    /// <summary>
    /// Get all the teachers from the database
    /// </summary>
    /// <returns>
    /// The list of all available teachers
    /// </returns>
    [HttpGet("teachers")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserModel>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GetTeachers()
    {
        try
        {
            IEnumerable<UserEntity> users = _userRepository
                .GetUsersByRole(TEACHER_ROLE)
                .Where(u => u.DeletedAt is null);

            return Ok(users);
        }
        catch (Exception e)
        {
            _logger.LogError(
                "{date} - An exception was thrown during \"{fn}\" :\n\"{eMsg}\"\n\"{eStr}\"",
                DateTime.Now,
                nameof(GetTeachers),
                e.Message,
                e.StackTrace
            );
#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest("An error occurred while trying to get teachers, please contact the administrator");
#endif
        }
    }

    /// <summary>
    /// Get the user by the given ID.
    /// </summary>
    /// <param name="id">The user id</param>
    /// <returns>
    /// Get the user information by the given ID.
    /// </returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Get(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest($"The id is invalid");
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
                "{date} - An exception was thrown during \"{fn}\" :\n\"{eMsg}\"\n\"{eStr}\"",
                DateTime.Now,
                nameof(Get),
                e.Message,
                e.StackTrace
            );
#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest("An error occurred while trying to get user, please contact the administrator");
#endif
        }
    }

    /// <summary>
    /// Updates a user with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the user to update.</param>
    /// <param name="form">The updated user information.</param>
    /// <returns>
    /// The updated user information.
    /// </returns>
    [HttpPut]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Put([FromBody] UserForm form, [FromQuery] string? id)
    {
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

            if (!string.IsNullOrEmpty(id))
            {
                if (!currentUserId.Equals(id))
                {
                    if (!_userManager.IsInRole(RolesData.DIRECTION_ROLE))
                    {
                        return Unauthorized("You are not authorized to modify other user's information");
                    }
                }
            }
            else
            {
                id = currentUserId;
            }

            UserEntity? userFromRepo = _userRepository.GetByKey(id);

            if (userFromRepo is null)
            {
                return NotFound($"The requested user does not exist");
            }

            _mapper.Map(form, userFromRepo);



            if (!_userRepository.Update(id, userFromRepo))
            {
                _logger.LogError("{dt} - Updating user \"{id}\" failed", DateTime.Now, id);

                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while trying to update user's information, please contact the administrator");
            }

            userFromRepo = _userRepository.GetByKey(id);

            UserModel user = _mapper.Map<UserModel>(userFromRepo);

            return Ok(user);
        }
        catch (Exception e)
        {
            _logger.LogError(
                "{dt} - An exception was thrown during \"{fun}\":\n{msg}\"\n{stack}",
                DateTime.Now,
                nameof(Put),
                e.Message,
                e.StackTrace);

#if DEBUG
            return BadRequest(e.Message);
#else
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                "An error occurred while trying to update user's information, please contact the administrator");
#endif
        }
    }

    /// <summary>
    /// Endpoint to upload a profile picture for a user.
    /// </summary>
    /// <param name="file">The profile picture file to upload.</param>
    /// <param name="userId">The ID of the user to associate the profile picture with (optional).</param>
    /// <returns>
    /// <see cref="StatusCodes.Status200OK"/> if the profile picture upload is successful.
    /// <see cref="StatusCodes.Status400BadRequest"/> if the file is null or missing.
    /// <see cref="StatusCodes.Status401Unauthorized"/> if the user is not authenticated or not authorized to perform the action.
    /// <see cref="StatusCodes.Status404NotFound"/> if the requested user does not exist.
    /// <see cref="StatusCodes.Status500InternalServerError"/> if there is an error during the upload or database operations.
    /// </returns>
    [HttpPost(nameof(UploadProfilePicture))]
    public async Task<IActionResult> UploadProfilePicture([FromForm] IFormFile file, [FromQuery] string? userId)
    {
        try
        {
            if (file is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while trying to update user's information, please contact the administrator");
            }

            string? currentUserId = _userManager.GetCurrentUserId;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized("You must be authenticated to perform this action");
            }

            if (!string.IsNullOrEmpty(userId))
            {
                if (!currentUserId.Equals(userId))
                {
                    if (!_userManager.IsInRole(RolesData.DIRECTION_ROLE))
                    {
                        return Unauthorized("You are not authorized to modify other user's information");
                    }
                }
            }
            else
            {
                userId = currentUserId;
            }

            UserEntity? userFromRepo = _userRepository.GetByKey(userId);

            if (userFromRepo is null)
            {
                return NotFound($"The requested user does not exist");
            }

            string fileName = Path.GetRandomFileName().Split('.')[0];

            if (!await _fileManager.UploadObjectAsync(file, fileName))
            {
                _logger.LogError(
                    "{dt} - Uploading the file for user \"{id}\" in AWS failed",
                    DateTime.Now,
                    userId
                );

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while trying to update user's information, please contact the administrator");
            }

            FileEntity fileEntity = new FileEntity()
            {
                Key = fileName,
                Type = file.ContentType,
                Size = file.Length
            };

            int createdFileId = _fileRepository.Create(fileEntity);

            if (createdFileId == 0)
            {
                _logger.LogError(
                    "{dt} - The file for user \"{id}\" was not saved in the database",
                    DateTime.Now,
                    userId
                );

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while trying to update user's information, please contact the administrator");
            }

            userFromRepo.ProfilePictureId = createdFileId;

            if (!_userRepository.Update(userId, userFromRepo))
            {
                _logger.LogError("{dt} - Updating user \"{id}\" failed",
                    DateTime.Now,
                    userId
                );

                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while trying to update user's information, please contact the administrator");
            }


            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(
                "{dt} - An exception was thrown during \"{fun}\":\n{msg}\"\n{stack}",
                DateTime.Now,
                nameof(UploadProfilePicture),
                e.Message,
                e.StackTrace);

#if DEBUG
            return BadRequest(e.Message);

#else
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                "An error occurred while trying to update user's profile picture, please contact the administrator");
#endif
        }
    }

    /// <summary>
    /// Endpoint to delete the user account, either the current user or the one specified in the query parameter.
    /// </summary>
    /// <param name="id">The id of the user to delete (optional)</param>
    /// <returns></returns>
    [Authorize]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(string? id = null)
    {
        try
        {
            string? userId = id;

            if (!string.IsNullOrEmpty(userId))
            {
                if (!_userManager.IsInRole(DIRECTION_ROLE))
                {
                    return Unauthorized("You are not authorized to delete other user's account");
                }
            }
            else
            {
                userId = _userManager.GetCurrentUserId;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("You must be authenticated to perform this action");
                }
            }

            UserEntity? userFromRepo = _userRepository.GetByKey(userId);

            if (userFromRepo is null)
            {
                return NotFound($"The requested user does not exist");
            }

            if (userFromRepo.DeletedAt is not null)
            {
                return BadRequest("The user account is already deleted");
            }

            bool deleteResult = _userRepository.Delete(userId);

            if (!deleteResult)
            {
                return BadRequest("The user account could not be deleted");
            }

            return NoContent();
        }
        catch (Exception e)
        {
            _logger.LogError(
                "{date} - An exception was thrown during \"{fn}\":\n{eMsg}\"\n{eStr}",
                DateTime.Now,
                nameof(UploadProfilePicture),
                e.Message,
                e.StackTrace
            );

#if DEBUG
            return BadRequest(e.Message);

#else
            return BadRequest("An error occurred while trying to delete the user account, please contact the administrator");
#endif
        }
    }

    /// <summary>
    /// Update user's password
    /// </summary>
    /// <param name="passwordUpdateForm"></param>
    /// <returns></returns>
    [HttpPost(nameof(UpdatePassword))]
    public IActionResult UpdatePassword([FromBody] PasswordUpdateForm passwordUpdateForm)
    {
        if (passwordUpdateForm is null)
        {
            return BadRequest("The password update form is null");
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
                return Unauthorized("User is not authenticated");
            }

            UserEntity? userFromRepo = _userRepository.GetByKey(currentUserId);

            if (userFromRepo is null)
            {
                return NotFound("The requested user does not exist");
            }

            if (userFromRepo.DeletedAt is not null)
            {
                return Forbid("The user is not active");
            }

            if (string.IsNullOrEmpty(userFromRepo.NormalizedEmail))
            {
                return BadRequest("The user does not have an email address");
            }

            UserEntity? loggedInUserFromRepo = _userRepository.LogIn(userFromRepo.NormalizedEmail, passwordUpdateForm.CurrentPassword, GetSalt());

            if (loggedInUserFromRepo is null)
            {
                return BadRequest("The current password is incorrect");
            }

            bool result = _userRepository.UpdatePassword(currentUserId, passwordUpdateForm.NewPassword, GetSalt());

            if (!result)
            {
                return BadRequest("The password could not be updated");
            }

            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError($"{DateTime.Now} - An exception was thrown during \"{nameof(UpdatePassword)}\" :\n" +
                $"\"{e.Message}\"\n\"{e.StackTrace}\"");

#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest("An error occurred while trying to update the password, please contact the administrator");
#endif
        }
    }

    /// <summary>
    /// Get all the addresses of the user with the given ID. Requires the user to be authenticated and to be in the 
    /// direction role.
    /// </summary>
    /// <param name="userId">The id of the user</param>
    /// <returns>
    /// The list of all addresses belonging to the user with the given ID.
    /// </returns>
    [HttpGet("{userId}/Addresses")]
    [HasRoleAuthorize(RolesEnum.DIRECTION)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<AddressModel>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetAddresses(string userId)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest($"The user ID is invalid");
        }

        try
        {
            IEnumerable<AddressEntity> addressesFromRepo = _addressRepository.GetAll().Where(a => a.UserId.Equals(userId));

            IEnumerable<AddressModel> addressesToReturn = _mapper.Map<IEnumerable<AddressModel>>(addressesFromRepo);

            return Ok(addressesToReturn);
        }
        catch (Exception e)
        {
            _logger.LogError(
                "{date} - An exception was thrown during \"{fn}\" :\n\"{eMsg}\"\n\"{eStr}\"",
                DateTime.Now,
                nameof(GetAddresses),
                e.Message,
                e.StackTrace
            );

#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest("An error occurred while trying to get user's addresses, please contact the administrator");
#endif
        }
    }

    /// <summary>
    /// Get the user roles by the given user ID
    /// </summary>
    /// <param name="userId">The id of the user</param>
    /// <returns>
    /// The list of all roles belonging to the user with the given ID.
    /// </returns>
    [HttpGet("{userId}/Roles")]
    [HasRoleAuthorize(RolesEnum.DIRECTION)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RoleModel>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetRoles(string userId)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest($"The user ID is invalid");
        }

        try
        {
            IEnumerable<RoleEntity> rolesFromRepo = _roleRepository.GetUserRoles(userId);

            IEnumerable<RoleModel> rolesToReturn = _mapper.Map<IEnumerable<RoleModel>>(rolesFromRepo);

            return Ok(rolesToReturn);
        }
        catch (Exception e)
        {
            _logger.LogError(
                "{date} - An exception was thrown during \"{fn}\" :\n\"{eMsg}\"\n\"{eStr}\"",
                DateTime.Now,
                nameof(GetRoles),
                e.Message,
                e.StackTrace
            );

#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest("An error occurred while trying to get user's roles, please contact the administrator");
#endif
        }
    }

    /// <summary>
    /// Update the user role by the given user ID and role ID
    /// </summary>
    /// <param name="userId">The id of the user</param>
    /// <param name="roleId">The id of the role</param>
    /// <returns>
    /// Nothing if the operation is successful. Otherwise, return an error
    /// </returns>
    [HttpPut("{userId}/Role/{roleId}")]
    [HasRoleAuthorize(RolesEnum.DIRECTION)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult UpdateUserRole(string userId, string roleId)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest($"The user ID is invalid");
        }

        if (string.IsNullOrEmpty(roleId) || string.IsNullOrWhiteSpace(roleId))
        {
            return BadRequest($"The role ID is invalid");
        }

        try
        {
            userId = userId.ToUpper();
            roleId = roleId.ToUpper();

            UserEntity? userFromRepo = _userRepository.GetByKey(userId);

            if (userFromRepo is null)
            {
                return NotFound($"The requested user does not exist");
            }

            if (userFromRepo.DeletedAt is not null)
            {
                return BadRequest($"The user is not active");
            }

            RoleEntity? roleFromRepo = _roleRepository.GetByKey(roleId);

            if (roleFromRepo is null)
            {
                return NotFound($"The requested role does not exist");
            }

            IEnumerable<RoleEntity> userRoles = _roleRepository.GetUserRoles(userId);

            foreach (var role in userRoles)
            {
                bool result = _roleRepository.RemoveUser(role.Id, userId);

                if (!result)
                {
                    return BadRequest($"The role \"{role.Name}\" could not be removed from the user");
                }
            }

            bool addResult = _roleRepository.AddUser(roleId, userId);

            if (!addResult)
            {
                return BadRequest($"The role \"{roleFromRepo.Name}\" could not be added to the user");
            }

            return NoContent();
        }
        catch (Exception e)
        {
            _logger.LogError(
                "{date} - An exception was thrown during \"{fn}\" :\n\"{eMsg}\"\n\"{eStr}\"",
                DateTime.Now,
                nameof(UpdateUserRole),
                e.Message,
                e.StackTrace
            );

#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest("An error occurred while trying to update user role, please contact the administrator");
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
