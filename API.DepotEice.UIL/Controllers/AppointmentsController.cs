using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.UIL.AuthorizationAttributes;
using API.DepotEice.UIL.Interfaces;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;using static API.DepotEice.UIL.Data.RolesData;
using static API.DepotEice.UIL.Data.Utils;

namespace API.DepotEice.UIL.Controllers;

/// <summary>
/// Appointment controller
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AppointmentsController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IUserManager _userManager;
    private readonly IOpeningHoursRepository _openingHoursRepository;
    private readonly IDateTimeManager _dateTimeManager;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="mapper"></param>
    /// <param name="appointmentRepository"></param>
    /// <param name="userManager"></param>
    /// <param name="openingHoursRepository"></param>
    /// <param name="dateTimeManager"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public AppointmentsController(ILogger<AppointmentsController> logger, IMapper mapper,
        IAppointmentRepository appointmentRepository, IUserManager userManager,
        IOpeningHoursRepository openingHoursRepository, IDateTimeManager dateTimeManager)
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

        if (openingHoursRepository is null)
        {
            throw new ArgumentNullException(nameof(openingHoursRepository));
        }

        if (dateTimeManager is null)
        {
            throw new ArgumentNullException(nameof(dateTimeManager));
        }

        _logger = logger;
        _mapper = mapper;
        _appointmentRepository = appointmentRepository;
        _userManager = userManager;
        _openingHoursRepository = openingHoursRepository;
        _dateTimeManager = dateTimeManager;
    }

    /// <summary>
    /// Get all the appointments, if the user is not in the direction role, the user id will be empty for all appointments
    /// except the ones that belong to the user
    /// </summary>
    /// <param name="date">The date and time</param>
    /// <param name="range">The range to select when selecting a date</param>
    /// <returns></returns>
    [HasRoleAuthorize(RolesEnum.GUEST, true)]
    [HttpGet]
    public IActionResult Get(DateTime? date, DateRange range = DateRange.Day)
    {
        try
        {
            string? currentUserId = _userManager.GetCurrentUserId;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized("You must be authenticated to perform this action");
            }

            var appointmentsFromRepo = _appointmentRepository.GetAll();

            var appointments = _mapper.Map<IEnumerable<AppointmentModel>>(appointmentsFromRepo);

            if (!_userManager.IsInRole(DIRECTION_ROLE))
            {
                foreach (var appointment in appointments)
                {
                    if (!appointment.UserId.Equals(currentUserId))
                    {
                        appointment.UserId = string.Empty;
                    }
                }
            }

            if (date.HasValue)
            {

                switch (range)
                {
                    case DateRange.Day:
                        appointments = appointments.Where(a =>
                            a.StartAt.Year == date.Value.Year &&
                            a.StartAt.Month == date.Value.Month &&
                            a.StartAt.Day == date.Value.Day);
                        break;
                    case DateRange.Week:
                        appointments = appointments.Where(a =>
                            a.StartAt.Year == date.Value.Year &&
                            a.StartAt.Month == date.Value.Month &&
                            (a.StartAt.Day < date.Value.AddDays(7).Day && a.StartAt.Day > date.Value.AddDays(-7).Day));
                        break;
                    case DateRange.Month:
                        appointments = appointments.Where(a =>
                            a.StartAt.Year == date.Value.Year &&
                            a.StartAt.Month == date.Value.Month);
                        break;
                    case DateRange.Year:
                        appointments = appointments.Where(a => a.StartAt.Year == date.Value.Year);
                        break;
                    default:
                        _logger.LogError("The date range is not valid");
                        break;
                }
            }

            return Ok(appointments);
        }
        catch (Exception ex)
        {
            _logger.LogError($"{DateTime.Now} - An exception was thrown during {nameof(Get)}.\"" +
                    $"{ex.Message}\n{ex.StackTrace}");
#if DEBUG
            return BadRequest(ex.Message);
#else
            return BadRequest("An error occurred while trying to get appointments, please contact the administrator");
#endif
        }
    }

    /// <summary>
    /// Get a specific appointment by id
    /// </summary>
    /// <param name="id">the id of the appointment</param>
    /// <returns></returns>
    [HasRoleAuthorize(RolesEnum.GUEST)]
    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            string? userId = _userManager.GetCurrentUserId;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("You must be authenticated to perform this action");
            }

            var appointmentFromRepo = _appointmentRepository.GetByKey(id);

            if (appointmentFromRepo is null)
            {
                return NotFound($"No appointment with ID {id} found");
            }

            var appointment = _mapper.Map<AppointmentModel>(appointmentFromRepo);

            if (!appointment.UserId.Equals(userId) && !_userManager.IsInRole(DIRECTION_ROLE))
            {
                return Unauthorized("You are not allowed to retrieve another user's appointment");
            }

            return Ok(appointment);
        }
        catch (Exception ex)
        {
            _logger.LogError($"{DateTime.Now} - An exception was thrown during {nameof(Get)}.\"" +
                   $"{ex.Message}\n{ex.StackTrace}");
#if DEBUG
            return BadRequest(ex.Message);
#else
            return BadRequest($"An error occurred while trying to get an appointment by its id \"{id}\", please contact the administrator");
#endif
        }
    }

    /// <summary>
    /// Create an appointment
    /// </summary>
    /// <param name="appointment"></param>
    /// <returns></returns>
    [HasRoleAuthorize(RolesEnum.GUEST)]
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
            if (!_dateTimeManager.DateTimeIsAvailable(appointment))
            {
                return BadRequest("The date and time you choose is not available");
            }

            if (_appointmentRepository.GetAll().Any(a => a.StartAt == appointment.StartAt))
            {
                return BadRequest("Hours not available");
            }

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

            var appointmentToReturn = _mapper.Map<AppointmentModel>(appointmentFromRepo);

            return Ok(appointmentToReturn);
        }
        catch (Exception ex)
        {
            _logger.LogError($"{DateTime.Now} - An exception was thrown during {nameof(Post)}.\"" +
                   $"{ex.Message}\n{ex.StackTrace}");
#if DEBUG
            return BadRequest(ex.Message);
#else
            return BadRequest("An error occurred while trying to create an appointment, please contact the administrator");
#endif
        }
    }

    /// <summary>
    /// Update an appointment
    /// </summary>
    /// <param name="id">The id of the appointment</param>
    /// <param name="appointment">The appointment form</param>
    /// <returns></returns>
    [HasRoleAuthorize(RolesEnum.GUEST)]
    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody] AppointmentForm appointment)
    {
        if (id <= 0)
        {
            return BadRequest("The ID must be greater than 0");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            if (!_dateTimeManager.DateTimeIsAvailable(appointment))
            {
                return BadRequest("The selected date and time is not available");
            }

            string? userId = _userManager.GetCurrentUserId;

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("You must be authenticated to perform this action");
            }

            AppointmentEntity? appointmentFromRepo = _appointmentRepository.GetByKey(id);

            if (appointmentFromRepo is null)
            {
                return NotFound("The appointment you are trying to update doesn't exist");
            }

            _mapper.Map(appointment, appointmentFromRepo);

            bool result = _appointmentRepository.Update(id, appointmentFromRepo);

            if (!result)
            {
                return BadRequest("The update of the appointment failed");
            }

            return Ok(appointmentFromRepo);
        }
        catch (Exception ex)
        {
            _logger.LogError($"{DateTime.Now} - An exception was thrown during {nameof(Put)}.\"" +
                   $"{ex.Message}\n{ex.StackTrace}");
#if DEBUG
            return BadRequest(ex.Message);
#else
            return BadRequest($"An error occurred while trying to update the appointment with ID \"{id}\", please contact the administrator");
#endif
        }
    }

    /// <summary>
    /// Delete an appointment from the database
    /// </summary>
    /// <param name="id">The id of the appointment to delete</param>
    /// <returns></returns>
    [HasRoleAuthorize(RolesEnum.DIRECTION)]
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (id <= 0)
        {
            return BadRequest("The ID must be greater than 0");
        }

        try
        {
            AppointmentEntity? appointmentFromRepo = _appointmentRepository.GetByKey(id);

            if (appointmentFromRepo is null)
            {
                return NotFound("The appointment you are trying to delete doesn't exist");
            }

            bool result = _appointmentRepository.Delete(id);

            if (!result)
            {
                return BadRequest("The delete failed");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"{DateTime.Now} - An exception was thrown during {nameof(Delete)}.\"" +
                   $"{ex.Message}\n{ex.StackTrace}");
#if DEBUG
            return BadRequest(ex.Message);
#else
            return BadRequest($"An error occurred while trying to delete the appointment with ID \"{id}\", please contact the administrator");
#endif
        }
    }

    [HasRoleAuthorize(RolesEnum.DIRECTION)]
    [HttpGet($"{nameof(Accept)}/{{id}}")]
    public IActionResult Accept(int id)
    {
        try
        {
            var result = _appointmentRepository.AppointmentDecision(id, true);

            if (!result)
            {
                return BadRequest();
            }

            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [HasRoleAuthorize(RolesEnum.DIRECTION)]
    [HttpGet($"{nameof(Cancel)}/{{id}}")]
    public IActionResult Cancel(int id)
    {
        try
        {
            var result = _appointmentRepository.AppointmentDecision(id, false);

            if (!result)
            {
                return BadRequest();
            }

            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }
}
