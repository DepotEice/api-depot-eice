using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.UIL.Models.Forms;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.DepotEice.UIL.Controllers;

// TODO : Implements methods

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IUserTokenRepository _userTokenRepository;
    private readonly IRoleRepository _roleRepository;

    public UsersController(
        IUserRepository userRepository,
        IUserTokenRepository userTokenRepository,
        IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _userTokenRepository = userTokenRepository;
        _roleRepository = roleRepository;
    }

    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{id}")]
    public IActionResult Get(string id)
    {
        return Ok();
    }

    [HttpPost]
    public IActionResult Post([FromBody] UserForm form)
    {
        return Ok();
    }

    [HttpPut("{id}")]
    public IActionResult Put(string id, [FromBody] UserForm form)
    {
        return Ok();
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
    /// <param name="token"></param>
    /// <returns></returns>
    [HttpPost(nameof(Password))]
    public IActionResult Password([FromBody] PasswordForm passwordForm, string? token = null)
    {
        if (passwordForm is null)
        {
            return BadRequest("The body cannot be null!");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (string.IsNullOrEmpty(token))
        {
            string? userId = User.Claims.SingleOrDefault(c => c.Type.Equals(ClaimTypes.Sid))?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (!userId.Equals(passwordForm.UserId))
            {
                return Unauthorized();
            }

            bool result = _userRepository.UpdatePassword(passwordForm.UserId, passwordForm.Password);

            if (!result)
            {
                return BadRequest("Password update failed!");
            }

            return Ok();
        }
        else
        {
            // TODO : Token password update
            // 1. Verify if token is valid
            // 2. Verify if token's user id is the same as the user asking the change
            // 3. Verify if user exist

            UserTokenEntity? userTokenFromRepo = _userTokenRepository
                .GetAll()
                .SingleOrDefault(ut => ut.Value.Equals(token));

            if (userTokenFromRepo is null)
            {
                return NotFound("Token doesn't exist");
            }

            if (!_userTokenRepository.ApproveToken(userTokenFromRepo))
            {
                return BadRequest("Token is invalid or expired");
            }

            if (!passwordForm.UserId.Equals(userTokenFromRepo.UserId))
            {
                return Unauthorized();
            }

            bool result = _userRepository.UpdatePassword(passwordForm.UserId, passwordForm.Password);

            if (!result)
            {
                return BadRequest("Password update failed");
            }

            return Ok();
        }
    }
}
