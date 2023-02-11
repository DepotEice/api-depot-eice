using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using API.DepotEice.UIL.AuthorizationAttributes;
using API.DepotEice.UIL.Interfaces;
using API.DepotEice.UIL.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;
using static API.DepotEice.UIL.Data.RolesData;

namespace API.DepotEice.UIL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IFileManager _fileManager;

        public ImagesController(ILogger<ImagesController> logger, IConfiguration configuration, IFileManager fileManager)
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

            _logger = logger;
            _configuration = configuration;
            _fileManager = fileManager;
        }

        [HttpGet("{fileName}")]
        public async Task<IActionResult> GetImageAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return BadRequest($"The provided fileName is empty or null");
            }

            try
            {
                FileModel? fileModel = await _fileManager.GetObjectAsync(fileName);

                if (fileModel is null)
                {
                    return NotFound($"Could not find any image with name : \"{fileName}\"");
                }

                return File(fileModel.Content, fileModel.ContentType);
            }
            catch (Exception e)
            {
                _logger.LogError($"{DateTime.Now} - An exception was thrown during \"{nameof(GetImageAsync)}\" :\n" +
                $"\"{e.Message}\"\n\"{e.StackTrace}\"");

#if DEBUG
                return BadRequest(e.Message);
#else
                return BadRequest("An error occurred while trying to get an image, please contact the administrator");
#endif
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveImage(IList<IFormFile> uploadFiles)
        {
            if (uploadFiles == null)
            {
                return BadRequest($"{nameof(uploadFiles)} is null");
            }

            foreach (var file in uploadFiles)
            {
                if (file.Length <= 0)
                {
                    return BadRequest($"{nameof(file)} is empty");
                }

                if (!await _fileManager.UploadFileAsync(file, file.FileName))
                {
                    _logger.LogWarning($"{DateTime.Now} - The file \"{file.FileName}\" couldn't be uploaded to " +
                        $"AWS");
                }
            }

            return Ok();
        }

        [HttpDelete("{fileName}")]
        public async Task<IActionResult> DeleteImageAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return BadRequest($"The provided file name is either null or empty");
            }

            try
            {
                if (!await _fileManager.DeleteObjectAsync(fileName))
                {
                    return BadRequest($"Couldn't delete the file with name :\"{fileName}\"");
                }

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"{DateTime.Now} - An exception was thrown during \"{nameof(DeleteImageAsync)}\" :\n" +
                $"\"{e.Message}\"\n\"{e.StackTrace}\"");

#if DEBUG
                return BadRequest(e.Message);
#else
                return BadRequest("An error occurred while trying to delete the image, please contact the administrator");
#endif
            }
        }
    }
}
