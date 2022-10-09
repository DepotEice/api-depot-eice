using API.DepotEice.DAL.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace API.DepotEice.UIL.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentRepository _appointmentRepository;

    public AppointmentsController(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok();
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        return Ok();
    }

    /// <summary>
    /// Creates an appointment.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>A newly created appointment.</returns>
    /// <response code="201">Returns the newly created item.</response>
    /// <response code="400">If the item is null.</response>
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Post([FromBody] string value)
    {
        return Ok();
    }

    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody] string value)
    {
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        return Ok();
    }
}
