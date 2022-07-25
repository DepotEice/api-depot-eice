using API.DepotEice.UIL.Models.Forms;
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
        public IActionResult SignIn([FromBody] LoginForm form)
        {
            throw new NotImplementedException();
        }

        [HttpPost(nameof(SignUp))]
        public IActionResult SignUp([FromBody] RegisterForm form)
        {
            return Ok();
        }

        [HttpPost("{email}/"+nameof(PasswordRequest))]
        public IActionResult PasswordRequest(string email)
        {
            return Ok();
        }
    }
}
