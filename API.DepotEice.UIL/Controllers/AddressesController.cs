using API.DepotEice.DAL.Entities;
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
            return BadRequest("An error occurred while trying to create an address, please contact the administrator");
#endif
        }
    }

    /// <summary>
    /// Update the address with the given ID with the new data in the form
    /// </summary>
    /// <param name="id">The ID of the address to update</param>
    /// <param name="form">The new data to insert</param>
    /// <returns>
    /// <see cref="StatusCodes.Status200OK"/> If everything went successfully.
    /// <see cref="StatusCodes.Status400BadRequest"/> If the provided ID or form are invalid or if the address couldn't
    /// be updated or if an error occurred while trying to performe the update.
    /// <see cref="StatusCodes.Status401Unauthorized"/> If the user is not logged in or if the address id is not the 
    /// currently logged in user's address
    /// <see cref="StatusCodes.Status404NotFound"/> If there is no address with the given ID
    /// </returns>
    [HttpPut("{id}")]
    public IActionResult UpdateAddress(int id, [FromBody] AddressForm form)
    {
        if (id < 0)
        {
            return BadRequest($"The provided id is a negative value");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            string? currentUserId = _userManager.GetCurrentUserId;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized($"The user requesting the update must be logged in");
            }

            AddressEntity? addressFromRepo = _addressRepository.GetByKey(id);

            if (addressFromRepo is null)
            {
                return NotFound($"There is no address with id \"{id}\"");
            }

            if (!addressFromRepo.UserId.Equals(currentUserId))
            {
                return Unauthorized($"You are not authorized to update another user's address");
            }

            AddressEntity addressToUpdate = _mapper.Map<AddressEntity>(form);

            addressToUpdate.Id = id;
            addressToUpdate.UserId = currentUserId;

            if (!_addressRepository.Update(id, addressToUpdate))
            {
                return BadRequest($"The address \"{id}\" couldn't be updated");
            }

            addressFromRepo = _addressRepository.GetByKey(id);

            return Ok(_mapper.Map<AddressModel>(addressFromRepo));
        }
        catch (Exception e)
        {
            _logger.LogError($"{DateTime.Now} - An exception was thrown during \"{nameof(UpdateAddress)}\" :\n" +
                $"\"{e.Message}\"\n\"{e.StackTrace}\"");

#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest("An error occurred while trying to update an address, please contact the administrator");
#endif
        }
    }
}
