using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.DepotEice.UIL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        public AuthController()
        {

        }

        [HttpPost(nameof(SignIn))]
        public IActionResult SignIn()
        {
            throw new NotImplementedException();
        }
    }
}
