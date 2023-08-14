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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        /// <param name="fileManager"></param>
        /// <param name="fileRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public FilesController(ILogger<FilesController> logger, IConfiguration configuration, IFileManager fileManager,
            IFileRepository fileRepository, IMapper mapper)
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

            _logger = logger;
            _configuration = configuration;
            _fileManager = fileManager;
            _fileRepository = fileRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetFiles()
        {
            try
            {
                var filesFromRepo = _fileRepository.GetAll();

                return Ok(filesFromRepo);
            }
            catch (Exception e)
            {
                _logger.LogError($"{DateTime.Now} - An exception was thrown during \"{nameof(GetFiles)}\" :\n" +
               $"\"{e.Message}\"\n\"{e.StackTrace}\"");

#if DEBUG
                return BadRequest(e.Message);
#else
                return BadRequest("An error occurred while trying to get files, please contact the administrator");
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
                _logger.LogError($"{DateTime.Now} - An exception was thrown during \"{nameof(GetDefaultProfilePictureAsync)}\" :\n" +
               $"\"{e.Message}\"\n\"{e.StackTrace}\"");

#if DEBUG
                return BadRequest(e.Message);
#else
                return BadRequest("An error occurred while trying to get the default profile picture file, please contact the administrator");
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
                _logger.LogError($"{DateTime.Now} - An exception was thrown during \"{nameof(GetFileAsync)}\" :\n" +
                    $"\"{e.Message}\"\n\"{e.StackTrace}\"");

#if DEBUG
                return BadRequest(e.Message);
#else
                return BadRequest("An error occurred while trying to get a file, please contact the administrator");
#endif
            }
        }

        /// <summary>
        /// Get a file by its name/key
        /// </summary>
        /// <param name="fileName">The name/key of the file</param>
        /// <returns>
        /// <see cref="StatusCodes.Status400BadRequest"/> If the name is null or an empty string or if the file is deleted.
        /// <br />
        /// <see cref="StatusCodes.Status404NotFound"/> IF the name doesn't retrieve any file from the database or if 
        /// the file key doesn't retrieve any file from the AWS S3 bucket.
        /// </returns>
        [HttpGet("ByFileName/{fileName}")]
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
                _logger.LogError($"{DateTime.Now} - An exception was thrown during \"{nameof(GetFileAsync)}\" :\n" +
                $"\"{e.Message}\"\n\"{e.StackTrace}\"");

#if DEBUG
                return BadRequest(e.Message);
#else
                return BadRequest("An error occurred while trying to get a file, please contact the administrator");
#endif
            }
        }

        /// <summary>
        /// Save a file to the database and to AWS S3 bucket
        /// </summary>
        /// <param name="uploadFiles">The files to upload</param>
        /// <returns>
        /// <see cref="StatusCodes.Status400BadRequest"/> If the provided file is null or empty or if the file couldn't be uploaded to AWS
        /// or if the file couldn't be saved to the database
        /// <br />
        /// <see cref="StatusCodes.Status200OK"/> If the file was successfully uploaded to AWS and saved to the database
        /// <br />
        /// <see cref="List{FileEntity}"/> The list of the saved files
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> SaveFileAsync(IList<IFormFile> uploadFiles)
        {
            if (uploadFiles is null)
            {
                return BadRequest($"{nameof(uploadFiles)} is null");
            }

            if (uploadFiles.Count <= 0)
            {
                return BadRequest($"{nameof(uploadFiles)} is empty");
            }

            try
            {
                List<FileEntity> files = new List<FileEntity>();

                foreach (var file in uploadFiles)
                {
                    if (file.Length <= 0)
                    {
                        return BadRequest($"{nameof(file)} is empty");
                    }

                    if (!await _fileManager.UploadObjectAsync(file, file.FileName))
                    {
                        _logger.LogWarning($"{DateTime.Now} - The file \"{file.FileName}\" couldn't be uploaded to " +
                            $"AWS");

                        return BadRequest($"The file \"{file.FileName}\" was not uploaded to AWS");
                    }

                    _logger.LogInformation($"{DateTime.Now} - The file \"{file.FileName}\" was successfully uploaded " +
                        $"to AWS");

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

                    files.Add(_fileFromRepo);
                }

                return Ok(files);
            }
            catch (Exception e)
            {
                _logger.LogError($"{DateTime.Now} - An exception was thrown during \"{nameof(SaveFileAsync)}\" :\n" +
                $"\"{e.Message}\"\n\"{e.StackTrace}\"");

#if DEBUG
                return BadRequest(e.Message);
#else
                return BadRequest("An error occurred while trying to save a file, please contact the administrator");
#endif
            }
        }

        /// <summary>
        /// Delete a file from the database and from AWS S3 bucket
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>
        /// <see cref="StatusCodes.Status204NoContent"/> If the file was successfully deleted from the database and from AWS S3 bucket
        /// <br />
        /// <see cref="StatusCodes.Status400BadRequest"/> If the provided file name is null or empty or if the file is already deleted
        /// or if the file couldn't be deleted from the database or from AWS S3 bucket
        /// <br />
        /// <see cref="StatusCodes.Status404NotFound"/> If the file couldn't be found in the database
        /// </returns>
        [HttpDelete("{fileName}")]
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
                _logger.LogError($"{DateTime.Now} - An exception was thrown during \"{nameof(DeleteFileAsync)}\" :\n" +
                $"\"{e.Message}\"\n\"{e.StackTrace}\"");

#if DEBUG
                return BadRequest(e.Message);
#else
                return BadRequest("An error occurred while trying to delete the file, please contact the administrator");
#endif
            }
        }
    }
}
