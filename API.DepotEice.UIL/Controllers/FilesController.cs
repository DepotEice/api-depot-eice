using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.UIL.AuthorizationAttributes;
using API.DepotEice.UIL.Data;
using API.DepotEice.UIL.Interfaces;
using API.DepotEice.UIL.Models;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;
using static API.DepotEice.UIL.Data.RolesData;

namespace API.DepotEice.UIL.Controllers
{
    /// <summary>
    /// The controller that manages the files
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IFileManager _fileManager;
        private readonly IFileRepository _fileRepository;
        private readonly IMapper _mapper;
        private readonly IUserManager _userManager;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        /// <param name="fileManager"></param>
        /// <param name="fileRepository"></param>
        /// <param name="mapper"></param>
        /// <param name="userManager"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public FilesController(ILogger<FilesController> logger, IConfiguration configuration, IFileManager fileManager,
            IFileRepository fileRepository, IMapper mapper, IUserManager userManager)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (fileManager is null)
            {
                throw new ArgumentNullException(nameof(fileManager));
            }

            if (fileRepository is null)
            {
                throw new ArgumentNullException(nameof(fileRepository));
            }

            if (mapper is null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            if (userManager is null)
            {
                throw new ArgumentNullException(nameof(userManager));
            }

            _logger = logger;
            _configuration = configuration;
            _fileManager = fileManager;
            _fileRepository = fileRepository;
            _mapper = mapper;
            _userManager = userManager;
        }

        /// <summary>
        /// Get all the files of the application
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetFiles()
        {
            try
            {
                string? userId = _userManager.GetCurrentUserId;

                IEnumerable<FileEntity> filesFromRepo = _fileRepository.GetAll();

                if (!string.IsNullOrEmpty(userId) && _userManager.IsInRole(TEACHER_ROLE))
                {
                    return Ok(filesFromRepo);
                }

                return Ok(filesFromRepo.Where(f => f.DeletedAt is not null));
            }
            catch (Exception e)
            {
                _logger.LogError(
                     "{date} - An exception was thrown during \"{fnName}\":\n{e.Message}\"\n\"{e.StackTrace}\"",
                     DateTime.Now,
                     nameof(GetFiles),
                     e.Message,
                     e.StackTrace
                 );
#if DEBUG
                return BadRequest(e.Message);
#else
                return BadRequest("An error occurred while trying to get all the files, please contact the administrator");
#endif
            }
        }

        /// <summary>
        /// Get the default profile picture file
        /// </summary>
        /// <returns>
        /// <see cref="StatusCodes.Status400BadRequest"/> If an error occurred while trying to get the default profile picture file.
        /// <br />
        /// <see cref="StatusCodes.Status404NotFound"/> If the default profile picture file is not found.
        /// <br />
        /// <see cref="StatusCodes.Status200OK"/> If the default profile picture file is found.
        /// <br />
        /// <see cref="FileContentResult"/> The default profile picture file if it exists.
        /// </returns>
        [HttpGet("DefaultProfilePicture")]
        public async Task<IActionResult> GetDefaultProfilePictureAsync()
        {
            try
            {
                FileModel? fileModel = await _fileManager.GetObjectAsync(Utils.DefaultProfilePicture);

                if (fileModel is null)
                {
                    return NotFound("The default profile picture file is not found");
                }

                return File(fileModel.Content, fileModel.ContentType, fileModel.FileName);
            }
            catch (Exception e)
            {
                _logger.LogError(
                     "{date} - An exception was thrown during \"{fnName}\":\n{e.Message}\"\n\"{e.StackTrace}\"",
                     DateTime.Now,
                     nameof(GetDefaultProfilePictureAsync),
                     e.Message,
                     e.StackTrace
                 );
#if DEBUG
                return BadRequest(e.Message);
#else
                return BadRequest("An error occurred while trying to get the default profile picture, please contact the administrator");
#endif
            }
        }

        /// <summary>
        /// Get a file by its ID
        /// </summary>
        /// <param name="id">The ID of the file</param>
        /// <returns>
        /// <see cref="StatusCodes.Status400BadRequest"/> If the ID is less than or equal to 0 or if the file is deleted.
        /// <br />
        /// <see cref="StatusCodes.Status404NotFound"/> IF the ID doesn't retrieve any file from the database or if 
        /// the file key doesn't retrieve any file from the AWS S3 bucket.
        /// <br />
        /// <see cref="StatusCodes.Status200OK"/> If the file is found.
        /// <br />
        /// <see cref="FileContentResult"/> The file if it exists.
        /// </returns>
        [HttpGet("ById/{id}")]
        public async Task<IActionResult> GetFileByIdAsync(int id)
        {
            if (id <= 0)
            {
                return BadRequest("The ID must be greater than 0");
            }

            try
            {
                FileEntity? fileFromRepo = _fileRepository.GetByKey(id);

                if (fileFromRepo is null)
                {
                    return NotFound("The file doesn't exist");
                }

                if (fileFromRepo.DeletedAt is not null)
                {
                    return BadRequest("The requested file is deleted");
                }

                FileModel? fileModel = await _fileManager.GetObjectAsync(fileFromRepo.Key);

                if (fileModel is null)
                {
                    return NotFound("The file is not found");
                }

                return File(fileModel.Content, fileModel.ContentType, fileModel.FileName);
            }
            catch (Exception e)
            {
                _logger.LogError(
                     "{date} - An exception was thrown during \"{fnName}\":\n{e.Message}\"\n\"{e.StackTrace}\"",
                     DateTime.Now,
                     nameof(GetFileByIdAsync),
                     e.Message,
                     e.StackTrace
                 );
#if DEBUG
                return BadRequest(e.Message);
#else
                return BadRequest("An error occurred while trying to get a file by its id, please contact the administrator");
#endif
            }
        }

        /// <summary>
        /// Get a file by its name/key
        /// </summary>
        /// <param name="fileName">The name/key of the file</param>
        /// <returns>
        /// The file if it exists.
        /// </returns>
        [HttpGet("ByFileName/{fileName}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileContentResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<IActionResult> GetFileAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return BadRequest($"The provided fileName is empty or null");
            }

            try
            {
                FileEntity? fileFromRepo = _fileRepository.GetAll().SingleOrDefault(f => f.Key.Equals(fileName));

                if (fileFromRepo is null)
                {
                    return NotFound("No file was found with the provided name");
                }

                if (fileFromRepo.DeletedAt is not null)
                {
                    return BadRequest("The requested file is deleted");
                }

                FileModel? fileModel = await _fileManager.GetObjectAsync(fileName);

                if (fileModel is null)
                {
                    return NotFound($"Could not find any image with name : \"{fileName}\"");
                }

                return File(fileModel.Content, fileModel.ContentType);
            }
            catch (Exception e)
            {
                _logger.LogError(
                     "{date} - An exception was thrown during \"{fnName}\":\n{e.Message}\"\n\"{e.StackTrace}\"",
                     DateTime.Now,
                     nameof(GetFileAsync),
                     e.Message,
                     e.StackTrace
                 );
#if DEBUG
                return BadRequest(e.Message);
#else
                return BadRequest("An error occurred while trying to get a file by its name, please contact the administrator");
#endif
            }
        }

        /// <summary>
        /// Save a file to the database and to AWS S3 bucket
        /// </summary>
        /// <param name="uploadFiles">The files to upload</param>
        /// <returns>
        /// The list of the created file IDs.
        /// </returns>
        [HttpPost]
        [HasRoleAuthorize(RolesEnum.TEACHER)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<int>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        public async Task<IActionResult> SaveFilesAsync([FromForm] IEnumerable<IFormFile>? uploadFiles)
        {
            if (uploadFiles is null)
            {
                return BadRequest($"{nameof(uploadFiles)} is null");
            }

            try
            {
                string? userId = _userManager.GetCurrentUserId;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("You must be authenticated to perform this action");
                }

                List<int> createdFileIds = new();

                foreach (var uploadFile in uploadFiles)
                {
                    if (uploadFile is null)
                    {
                        return BadRequest($"{nameof(uploadFile)} is null");
                    }

                    if (uploadFile.Length <= 0)
                    {
                        return BadRequest($"{nameof(uploadFile)} is empty");
                    }

                    if (!await _fileManager.UploadObjectAsync(uploadFile, uploadFile.FileName))
                    {
                        _logger.LogWarning($"{DateTime.Now} - The file \"{uploadFile.FileName}\" couldn't be uploaded to " +
                            $"AWS");

                        return BadRequest($"The file \"{uploadFile.FileName}\" was not uploaded to AWS");
                    }

                    _logger.LogInformation($"{DateTime.Now} - The file \"{uploadFile.FileName}\" was successfully uploaded " +
                        $"to AWS");

                    FileEntity fileEntity = _mapper.Map<FileEntity>(uploadFile);

                    int createdFileId = _fileRepository.Create(fileEntity);

                    if (createdFileId <= 0)
                    {
                        _logger.LogWarning($"{DateTime.Now} - The file \"{uploadFile.FileName}\" couldn't be saved to " +
                            $"the database");
                        return BadRequest($"The file \"{uploadFile.FileName}\" was not saved to the database");
                    }

                    _logger.LogInformation($"{DateTime.Now} - The file \"{uploadFile.FileName}\" was successfully saved " +
                        $"to the database");

                    FileEntity? _fileFromRepo = _fileRepository.GetByKey(createdFileId);

                    if (_fileFromRepo is null)
                    {
                        _logger.LogWarning($"{DateTime.Now} - The file \"{uploadFile.FileName}\" couldn't be retrieved " +
                                                       $"from the database");
                        return BadRequest($"The file \"{uploadFile.FileName}\" was not retrieved from the database");
                    }

                    createdFileIds.Add(createdFileId);
                }

                return Ok(createdFileIds);
            }
            catch (Exception e)
            {
                _logger.LogError(
                     "{date} - An exception was thrown during \"{fnName}\":\n{e.Message}\"\n\"{e.StackTrace}\"",
                     DateTime.Now,
                     nameof(SaveFilesAsync),
                     e.Message,
                     e.StackTrace
                 );
#if DEBUG
                return BadRequest(e.Message);
#else
                return BadRequest("An error occurred while trying to save a file, please contact the administrator");
#endif
            }
        }

        /// <summary>
        /// Upload a file for the Radzen HTML editor
        /// </summary>
        /// <param name="file"></param>
        /// <returns>
        /// Object containing the url
        /// </returns>
        [HttpPost("Article")]
        [HasRoleAuthorize(RolesEnum.DIRECTION)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        public async Task<IActionResult> UploadFileHTMLEditorAsync([FromForm] IFormFile file)
        {
            try
            {
                if (file is null)
                {
                    return BadRequest($"{nameof(file)} is null");
                }

                if (file.Length <= 0)
                {
                    return BadRequest($"{nameof(file)} is empty");
                }

                string? userId = _userManager.GetCurrentUserId;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("You must be authenticated to perform this action");
                }

                string fileName = $"{userId}-{new Random().Next(0, 10000)}-{file.FileName}";

                if (!await _fileManager.UploadObjectAsync(file, fileName))
                {
                    _logger.LogWarning(
                        "{date} - The file \"{name}\" couldn't be uploaded to AWS",
                        DateTime.Now,
                        fileName
                    );

                    return BadRequest($"The file \"{file.FileName}\" was not uploaded to AWS");
                }

                _logger.LogInformation(
                    "{date} - The file \"{fileName}\" was successfully uploaded to AWS",
                    DateTime.Now,
                    fileName
                );

                FileEntity fileEntity = _mapper.Map<FileEntity>(file);

                int createdFileId = _fileRepository.Create(fileEntity);

                if (createdFileId <= 0)
                {
                    _logger.LogWarning($"{DateTime.Now} - The file \"{file.FileName}\" couldn't be saved to " +
                        $"the database");

                    return BadRequest($"The file \"{file.FileName}\" was not saved to the database");
                }

                _logger.LogInformation($"{DateTime.Now} - The file \"{file.FileName}\" was successfully saved " +
                    $"to the database");

                FileEntity? _fileFromRepo = _fileRepository.GetByKey(createdFileId);

                if (_fileFromRepo is null)
                {
                    _logger.LogWarning($"{DateTime.Now} - The file \"{file.FileName}\" couldn't be retrieved " +
                        $"from the database");

                    return BadRequest($"The file \"{file.FileName}\" was not retrieved from the database");
                }


                var fileModel = await _fileManager.GetObjectAsync(fileName);

                if (fileModel is null)
                {
                    return NotFound("The file was not found");
                }

                return Ok(new { Url = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/api/Files/ById/{createdFileId}" });
            }
            catch (Exception e)
            {
                _logger.LogError(
                     "{date} - An exception was thrown during \"{fnName}\":\n{e.Message}\"\n\"{e.StackTrace}\"",
                     DateTime.Now,
                     nameof(UploadFileHTMLEditorAsync),
                     e.Message,
                     e.StackTrace
                 );
#if DEBUG
                return BadRequest(e.Message);
#else
                return BadRequest("An error occurred while trying to save a file for the Radzen HTML editor, please contact the administrator");
#endif
            }
        }

        /// <summary>
        /// Delete a file by its id from the database and AWS
        /// </summary>
        /// <param name="fileId">
        /// The id of the file to delete
        /// </param>
        /// <returns></returns>
        [HttpDelete("/ByFileId/{fileId}")]
        [HasRoleAuthorize(RolesEnum.TEACHER)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<IActionResult> DeleteFileAsync(int fileId)
        {
            if (fileId <= 0)
            {
                return BadRequest($"The provided file id is invalid");
            }

            try
            {
                FileEntity? fileEntity = _fileRepository.GetByKey(fileId);

                if (fileEntity is null)
                {
                    return NotFound($"No file was found with id : \"{fileId}\"");
                }

                if (fileEntity.DeletedAt is not null)
                {
                    return BadRequest($"The file with id : \"{fileId}\" is already deleted");
                }

                if (!await _fileManager.DeleteObjectAsync(fileEntity.Key))
                {
                    return BadRequest($"Couldn't delete the file with id :\"{fileId}\"");
                }

                if (!_fileRepository.Delete(fileEntity.Id))
                {
                    return BadRequest($"Couldn't delete the file with id :\"{fileId}\"");
                }

                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(
                     "{date} - An exception was thrown during \"{fnName}\":\n{e.Message}\"\n\"{e.StackTrace}\"",
                     DateTime.Now,
                     nameof(DeleteFileAsync),
                     e.Message,
                     e.StackTrace
                 );
#if DEBUG
                return BadRequest(e.Message);
#else
                return BadRequest("An error occurred while trying to delete a file by its id, please contact the administrator");
#endif
            }
        }

        /// <summary>
        /// Delete a file from the database and from AWS S3 bucket
        /// </summary>
        /// <param name="fileName">The name of the file to delete</param>
        /// <returns></returns>
        [HttpDelete("/ByFileName/{fileName}")]
        [HasRoleAuthorize(RolesEnum.TEACHER)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<IActionResult> DeleteFileAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return BadRequest($"The provided file name is either null or empty");
            }

            try
            {
                FileEntity? fileEntity = _fileRepository.GetAll().SingleOrDefault(f => f.Key.Equals(fileName));

                if (fileEntity is null)
                {
                    return NotFound($"No file was found with name : \"{fileName}\"");
                }

                if (fileEntity.DeletedAt is not null)
                {
                    return BadRequest($"The file with name : \"{fileName}\" is already deleted");
                }

                if (!await _fileManager.DeleteObjectAsync(fileName))
                {
                    return BadRequest($"Couldn't delete the file with name :\"{fileName}\"");
                }

                if (!_fileRepository.Delete(fileEntity.Id))
                {
                    return BadRequest($"Couldn't delete the file with name :\"{fileName}\"");
                }

                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(
                     "{date} - An exception was thrown during \"{fnName}\":\n{e.Message}\"\n\"{e.StackTrace}\"",
                     DateTime.Now,
                     nameof(DeleteFileAsync),
                     e.Message,
                     e.StackTrace
                 );
#if DEBUG
                return BadRequest(e.Message);
#else
                return BadRequest("An error occurred while trying to delete a file by its file name, please contact the administrator");
#endif
            }
        }
    }
}
