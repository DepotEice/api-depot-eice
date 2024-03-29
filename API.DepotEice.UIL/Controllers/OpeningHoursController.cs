﻿using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.UIL.AuthorizationAttributes;
using API.DepotEice.UIL.Interfaces;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using static API.DepotEice.UIL.Data.RolesData;

namespace API.DepotEice.UIL.Controllers;

/// <summary>
/// Opening hours controller
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class OpeningHoursController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IOpeningHoursRepository _openingHoursRepository;
    private readonly IDateTimeManager _dateTimeManager;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="mapper"></param>
    /// <param name="openingHoursRepository"></param>
    /// <param name="dateTimeManager"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public OpeningHoursController(ILogger<OpeningHoursController> logger, IMapper mapper,
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
        _openingHoursRepository = openingHoursRepository;
        _dateTimeManager = dateTimeManager;
    }

    /// <summary>
    /// Get the opening hours, you can filter by day, month and year
    /// </summary>
    /// <param name="day">The day of the month</param>
    /// <param name="month">The month</param>
    /// <param name="year">The year</param>
    /// <returns></returns>
    [HttpGet]
    public IActionResult Get(int? day = null, int? month = null, int? year = null)
    {
        var openingHours = _mapper.Map<IEnumerable<OpeningHoursModel>>(_openingHoursRepository.GetAll());

        if (day.HasValue)
        {
            openingHours = openingHours.Where(oh => oh.OpenAt.Day == day.Value);
        }

        if (month.HasValue)
        {
            openingHours = openingHours.Where(oh => oh.OpenAt.Month == month.Value);
        }

        if (year.HasValue)
        {
            openingHours = openingHours.Where(oh => oh.OpenAt.Year == year.Value);
        }

        return Ok(openingHours);
    }

    /// <summary>
    /// Retrieves the opening hour by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var openingHour = _mapper.Map<OpeningHoursModel>(_openingHoursRepository.GetByKey(id));

        return Ok(openingHour);
    }

    /// <summary>
    /// Creates a new opening hour
    /// </summary>
    /// <param name="openingHours"></param>
    /// <returns></returns>
    [HasRoleAuthorize(RolesEnum.DIRECTION)]
    [HttpPost]
    public IActionResult Post([FromBody] OpeningHoursForm openingHours)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            if (!_dateTimeManager.OpeningHoursAvailable(openingHours))
            {
                return BadRequest();
            }

            var openingHoursEntity = _mapper.Map<OpeningHoursEntity>(openingHours);

            var createdId = _openingHoursRepository.Create(openingHoursEntity);

            if (createdId <= 0)
            {
                return BadRequest();
            }

            var openingHoursFromRepo = _openingHoursRepository.GetByKey(createdId);

            return Ok(_mapper.Map<OpeningHoursModel>(openingHoursFromRepo));
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    /// <summary>
    /// Updates an opening hour
    /// </summary>
    /// <param name="id"></param>
    /// <param name="openingHours"></param>
    /// <returns></returns>
    [HasRoleAuthorize(RolesEnum.DIRECTION)]
    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody] OpeningHoursForm openingHours)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            if (!_dateTimeManager.OpeningHoursAvailable(openingHours, id))
            {
                return BadRequest();
            }

            var openinghoursFromRepo = _openingHoursRepository.GetByKey(id);

            if (openinghoursFromRepo is null)
            {
                return NotFound();
            }

            _mapper.Map(openingHours, openinghoursFromRepo);

            var result = _openingHoursRepository.Update(id, openinghoursFromRepo);

            if (!result)
            {
                return BadRequest();
            }

            var openingHoursUpdated = _mapper.Map<OpeningHoursModel>(_openingHoursRepository.GetByKey(id));

            return Ok(openingHoursUpdated);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    /// <summary>
    /// Deletes an opening hour
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HasRoleAuthorize(RolesEnum.DIRECTION)]
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        try
        {
            if (!_openingHoursRepository.Delete(id))
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
