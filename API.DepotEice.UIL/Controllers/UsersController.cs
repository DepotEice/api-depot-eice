﻿using API.DepotEice.DAL.IRepositories;
using API.DepotEice.UIL.Models.Forms;
using Microsoft.AspNetCore.Mvc;

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

        }
    }
}
