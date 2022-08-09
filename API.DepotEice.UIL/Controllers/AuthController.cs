using API.DepotEice.BLL.IServices;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.DepotEice.UIL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public AuthController(IUserService userService, IRoleService roleService)
        {
            _userService = userService;
            _roleService = roleService;
        }

        [HttpPost(nameof(SignIn))]
        public IActionResult SignIn([FromBody] LoginForm form)
        {
            var LoggedInUser = new LoggedInUserModel()
            {

            };

            throw new NotImplementedException();
        }

        [HttpPost(nameof(SignUp))]
        public IActionResult SignUp([FromBody] RegisterForm form)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(form);
            }

            // TODO :
            // 1. Check if email exist
            // 2. Create user
            // 3. Add user to guest role
            // 4. Return created user

            if (_userService.EmailExist(form.Email))
            {
                return BadRequest("There is already an account with this email!");
            }

            return Ok();
        }

        [HttpPost("{email}/" + nameof(PasswordRequest))]
        public IActionResult PasswordRequest(string email)
        {
            return Ok();
        }

        [HttpPost(nameof(Activate))]
        public IActionResult Activate(string id, string token)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(id);
            }

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(token);
            }

            return Ok();
        }
    }
}
