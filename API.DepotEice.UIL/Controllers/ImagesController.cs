using API.DepotEice.UIL.AuthorizationAttributes;
using API.DepotEice.UIL.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using static API.DepotEice.UIL.Data.RolesData;

namespace API.DepotEice.UIL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly HttpClient _httpClient;

        public ImagesController(ILogger<ImagesController> logger, IWebHostEnvironment hostEnvironment)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (hostEnvironment is null)
            {
                throw new ArgumentNullException(nameof(hostEnvironment));
            }


            _logger = logger;
            _hostEnvironment = hostEnvironment;
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri("http://freeimage.host/api/1/upload/")
            };
        }

        [HttpGet("{fileName}")]
        public async Task<IActionResult> GetImage(string fileName)
        {
            Account account = new Account("dhea8umqv", "872675634599566", "RZlkP5LQs1WLXmueNw8iMlh8z_E");

            Cloudinary cloudinary = new Cloudinary(account);

            GetResourceResult result = cloudinary.GetResource(fileName);

            string extension = fileName.Split('.').Last();

            using (var webClient = new WebClient())
            {
                byte[] imageBytes = webClient.DownloadData(result.Url);
                return File(imageBytes, $"image/{extension}");
            }
        }

        [HasRoleAuthorize(RolesEnum.DIRECTION)]
        [HttpPost]
        public async Task<IActionResult> SaveImage(IList<IFormFile> uploadFiles)
        {
            if (uploadFiles == null)
            {
                return BadRequest($"{nameof(uploadFiles)} is null");
            }

            Account account = new Account("dhea8umqv", "872675634599566", "RZlkP5LQs1WLXmueNw8iMlh8z_E");

            Cloudinary cloudinary = new Cloudinary(account);

            foreach (var file in uploadFiles)
            {
                if (file.Length <= 0)
                {
                    return BadRequest($"{nameof(file)} is empty");
                }


                Stream stream = file.OpenReadStream();

                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, stream),
                    PublicId = file.FileName
                };

                var uploadResult = cloudinary.Upload(uploadParams);
            }


            return Ok();
        }
    }
}
