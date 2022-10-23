using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.UIL.AuthorizationAttributes;
using API.DepotEice.UIL.Interfaces;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using static API.DepotEice.UIL.Data.RolesData;

namespace API.DepotEice.UIL.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AppointmentsController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IUserManager _userManager;

    public AppointmentsController(ILogger<AppointmentsController> logger, IMapper mapper,
        IAppointmentRepository appointmentRepository, IUserManager userManager)
    {
        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        if (mapper is null)
        {
            throw new ArgumentNullException(nameof(mapper));
        }

        if (appointmentRepository is null)
        {
            throw new ArgumentNullException(nameof(appointmentRepository));
        }

        if (userManager is null)
        {
            throw new ArgumentNullException(nameof(userManager));
        }

        _logger = logger;
        _mapper = mapper;
        _appointmentRepository = appointmentRepository;
        _userManager = userManager;
    }

    [HasRoleAuthorize(RolesEnum.DIRECTION, false)]
    [HttpGet]
    public IActionResult Get()
    {
        var appointments = _mapper.Map<IEnumerable<AppointmentModel>>(_appointmentRepository.GetAll());

        return Ok(appointments);
    }

    [HasRoleAuthorize(RolesEnum.GUEST)]
    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }

        string? userId = _userManager.GetCurrentUserId;

        if (string.IsNullOrEmpty(userId))
        {
            return NotFound();
        }

        var appointments = _mapper.Map<IEnumerable<AppointmentModel>>(_appointmentRepository
            .GetAll()
            .Where(a => a.UserId.Equals(userId)));

        return Ok(appointments);
    }

    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Post([FromBody] AppointmentForm appointment)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var appointmentEntity = _mapper.Map<AppointmentEntity>(appointment);

            string? userid = _userManager.GetCurrentUserId;

            if (string.IsNullOrEmpty(userid))
            {
                return BadRequest();
            }

            appointmentEntity.UserId = userid;

            var id = _appointmentRepository.Create(appointmentEntity);

            if (id <= 0)
            {
                return BadRequest();
            }

            var appointmentFromRepo = _appointmentRepository.GetByKey(id);

            return Ok(_mapper.Map<AppointmentModel>(appointmentFromRepo));
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody] AppointmentForm appointment)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        try
        {
            string? userId = _userManager.GetCurrentUserId;

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest();
            }

            var appointmentFromRepo = _appointmentRepository.GetByKey(id);

            if (appointmentFromRepo is null)
            {
                return NotFound();
            }

            appointmentFromRepo.UserId = userId;

            _mapper.Map(appointment, appointmentFromRepo);

            var result = _appointmentRepository.Update(id, appointmentFromRepo);

            if (!result)
            {
                return BadRequest();
            }

            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        try
        {
            var result = _appointmentRepository.Delete(id);

            if (!result)
            {
                return BadRequest();
            }

            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }

        return Ok();
    }
}
