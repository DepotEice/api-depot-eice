﻿using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.UIL.Interfaces;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.DepotEice.UIL.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AddressesController : ControllerBase
{
    private ILogger _logger;
    private IMapper _mapper;
    private IAddressRepository _addressRepository;
    private IUserManager _userManager;

    public AddressesController(ILogger<AddressesController> logger, IMapper mapper,
        IAddressRepository addressRepository, IUserManager userManager)
    {
        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        if (mapper is null)
        {
            throw new ArgumentNullException(nameof(mapper));
        }

        if (addressRepository is null)
        {
            throw new ArgumentNullException(nameof(addressRepository));
        }

        if (userManager is null)
        {
            throw new ArgumentNullException(nameof(userManager));
        }

        _logger = logger;
        _mapper = mapper;
        _addressRepository = addressRepository;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult GetAddresses()
    {
        IEnumerable<AddressEntity> addressesFromRepo = _addressRepository.GetAll();

        return Ok(_mapper.Map<IEnumerable<AddressModel>>(addressesFromRepo));
    }

    [HttpGet("{id}")]
    public IActionResult GetAddress(int id)
    {
        AddressEntity? addressFromRepo = _addressRepository.GetByKey(id);

        if (addressFromRepo is null)
        {
            return NotFound($"Could not find any address with id : \"{id}\"");
        }

        return Ok(_mapper.Map<AddressModel>(addressFromRepo));
    }

    /// <summary>
    /// Create a new address
    /// </summary>
    /// <param name="form">The address form to create</param>
    /// <returns>
    /// <see cref="StatusCodes.Status200OK"/> If the creation was successful
    /// <see cref="StatusCodes.Status400BadRequest"/> If the form is invalid or if the address couldn't be created
    /// <see cref="StatusCodes.Status401Unauthorized"/> If the user creating the address is not logged in
    /// <see cref="StatusCodes.Status404NotFound"/> If the created address couldn't be retrieved
    /// </returns>
    [HttpPost]
    public IActionResult CreateAddress(AddressForm form)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            AddressEntity addressToCreate = _mapper.Map<AddressEntity>(form);

            string? currentUserId = _userManager.GetCurrentUserId;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized($"You must be logged in to create an address");
            }

            addressToCreate.UserId = currentUserId;

            int addressId = _addressRepository.Create(addressToCreate);

            if (addressId < 0)
            {
                return BadRequest($"The address couldn't be created");
            }

            AddressEntity? addressFromRepo = _addressRepository.GetByKey(addressId);

            if (addressFromRepo is null)
            {
                return NotFound($"The newly created address doesn't exist");
            }

            return Ok(_mapper.Map<AddressModel>(addressFromRepo));
        }
        catch (Exception e)
        {
            _logger.LogError($"{DateTime.Now} - An exception was thrown during \"{nameof(CreateAddress)}\" :\n" +
                $"\"{e.Message}\"\n\"{e.StackTrace}\"");

#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest("An error occurred while trying to creqte an address, please contact the administrator");
#endif
        }
    }
}
