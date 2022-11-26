using API.DepotEice.UIL.AuthorizationAttributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static API.DepotEice.UIL.Data.RolesData;

namespace API.DepotEice.UIL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _hostEnvironment;

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
        }

        [HttpGet("{fileName}")]
        public async Task<IActionResult> GetImage(string fileName)
        {
            FileInfo infos = new FileInfo($"./images/{fileName}");

            if (!infos.Exists)
            {
                return NotFound(fileName);
            }

            byte[] bytes = System.IO.File.ReadAllBytes($"./images/{fileName}");

            return File(bytes, $"image/{infos.Extension}");
        }

        [HasRoleAuthorize(RolesEnum.DIRECTION)]
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

                if (!Directory.Exists("./images"))
                {
                    Directory.CreateDirectory("./images");
                }

                using (Stream fileStream = new FileStream($"./images/{file.FileName}", FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }



            return Ok();
        }
    }
}
