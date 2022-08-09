﻿using API.DepotEice.BLL.IServices;
using API.DepotEice.BLL.Models;
using API.DepotEice.UIL.Data;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.DepotEice.UIL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public AuthController(ILogger<AuthController> logger, IMapper mapper,
            IUserService userService, IRoleService roleService)
        {
            _logger = logger;
            _mapper = mapper;
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

            if (_userService.EmailExist(form.Email))
            {
                return BadRequest("There is already an account with this email!");
            }

            UserDto? createdUser = _userService.CreateUser(_mapper.Map<UserDto>(form));

            if (createdUser is null)
            {
                return BadRequest("User creation failed");
            }

            RoleDto? guestRole = _roleService.GetRoleByName(RolesData.GUEST_ROLE);

            if (guestRole is null)
            {
                guestRole = _roleService.CreateRole(new RoleDto()
                {
                    Name = RolesData.GUEST_ROLE
                });

                if (guestRole is null)
                {
                    return NoContent();
                }
            }

            if (!_roleService.AddUser(createdUser.Id, guestRole.Id))
            {
                // TODO : Return an error message with explanation of what happened
                return NoContent();
            }

            return Ok(createdUser);
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
