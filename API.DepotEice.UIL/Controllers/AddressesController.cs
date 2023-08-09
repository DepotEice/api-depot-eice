using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.UIL.AuthorizationAttributes;
using API.DepotEice.UIL.Interfaces;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static API.DepotEice.UIL.Data.RolesData;

namespace API.DepotEice.UIL.Controllers;

/// <summary>
/// The addresses controller
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AddressesController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IAddressRepository _addressRepository;
    private readonly IUserManager _userManager;

    /// <summary>
    /// Instanciate the Addresses controller
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="mapper"></param>
    /// <param name="addressRepository"></param>
    /// <param name="userManager"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public AddressesController(ILogger<AddressesController> logger, IMapper mapper, IAddressRepository addressRepository,
        IUserManager userManager)
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

    /// <summary>
    /// Get all addresses belonging to the currently authenticated user.
    /// </summary>
    /// <returns>
    /// <see cref="StatusCodes.Status200OK"/> if the operation is successful and returns the addresses.
    /// <see cref="StatusCodes.Status401Unauthorized"/> if the user requesting the retrieval is not logged in.
    /// </returns>
    [HttpGet]
    public IActionResult GetAddresses()
    {
        // Get the ID of the currently authenticated user
        string? currentUserId = _userManager.GetCurrentUserId;

        // Check if the user is logged in
        if (string.IsNullOrEmpty(currentUserId))
        {
            return Unauthorized($"The user requesting the retrieval must be logged in");
        }

        // Retrieve all addresses from the repository
        IEnumerable<AddressEntity> addressesFromRepo = _addressRepository.GetAll();

        // Map the addresses to the corresponding model
        IEnumerable<AddressModel> addresses = _mapper.Map<IEnumerable<AddressModel>>(addressesFromRepo);

        // Filter the addresses to only include those belonging to the current user
        var userAddresses = addresses.Where(a => a.UserId.Equals(currentUserId));

        return Ok(userAddresses);
    }


    /// <summary>
    /// Get a user's addresses. Only allowed to the direction members
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    /// <returns>
    /// <see cref="StatusCodes.Status200OK"/> If everything went successfully
    /// <see cref="StatusCodes.Status400BadRequest"/> If an error occurred during the process
    /// </returns>
    [HttpGet("User/{userId}")]
    [HasRoleAuthorize(RolesEnum.DIRECTION)]
    public IActionResult GetAddresses(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest($"Please provide a correct value for the user id");
        }

        try
        {
            IEnumerable<AddressEntity> addressesFromRepo = _addressRepository.GetAll();

            IEnumerable<AddressModel> addresses = _mapper.Map<IEnumerable<AddressModel>>(addressesFromRepo);

            return Ok(addresses.Where(a => a.UserId.Equals(userId)));
        }
        catch (Exception e)
        {
            _logger.LogError($"{DateTime.Now} - An exception was thrown during \"{nameof(GetAddresses)}\" :\n" +
                $"\"{e.Message}\"\n\"{e.StackTrace}\"");
#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest("An error occurred while trying to get user's addresses, please contact the administrator");
#endif
        }
    }

    /// <summary>
    /// Get the address with the given ID
    /// </summary>
    /// <param name="id">The ID of the address to retrieve</param>
    /// <returns>
    /// <see cref="StatusCodes.Status200OK"/> If everything went successfully
    /// <see cref="StatusCodes.Status401Unauthorized"/> If the user try to retrieve an address that is not his and is not
    /// in role DIRECTION
    /// <see cref="StatusCodes.Status404NotFound"/> If there is no address with the given ID
    /// </returns>
    [HttpGet("{id}")]
    public IActionResult GetAddress(int id)
    {
        string? currentUserId = _userManager.GetCurrentUserId;

        if (string.IsNullOrEmpty(currentUserId))
        {
            return Unauthorized($"The user requesting the address must be logged in!");
        }

        try
        {
            AddressEntity? addressFromRepo = _addressRepository.GetByKey(id);

            if (addressFromRepo is null)
            {
                return NotFound($"Could not find any address with id : \"{id}\"");
            }

            if (!_userManager.IsDirection)
            {
                if (!addressFromRepo.UserId.Equals(currentUserId))
                {
                    return Unauthorized($"The currently logged in user is not allowed to retrieve another " +
                        $"user's address");
                }
            }

            return Ok(_mapper.Map<AddressModel>(addressFromRepo));
        }
        catch (Exception e)
        {
            _logger.LogError($"{DateTime.Now} - An exception was thrown during \"{nameof(GetAddress)}\" :\n" +
                $"\"{e.Message}\"\n\"{e.StackTrace}\"");
#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest("An error occurred while trying to get a user's address, please contact the administrator");
#endif
        }
    }

    /// <summary>
    /// Create a new address
    /// </summary>
    /// <param name="form">The address form to create</param>
    /// <returns>
    /// <see cref="StatusCodes.Status200OK"/> If the creation was successful
    /// <see cref="StatusCodes.Status400BadRequest"/> If the form is invalid or if the address couldn't be created or
    /// if the old primary address couldn't be set to false
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

            bool primaryAddressExists = _addressRepository
                .GetAll()
                .Where(a => a.UserId.Equals(currentUserId) && a.IsPrimary)
                .Any();

            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized($"You must be logged in to create an address");
            }

            addressToCreate.UserId = currentUserId;

            if (!primaryAddressExists)
            {
                addressToCreate.IsPrimary = true;
            }
            else
            {
                addressToCreate.IsPrimary = false;
            }

            int addressId = _addressRepository.Create(addressToCreate);

            if (addressId <= 0)
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
    /// Set the address with the given ID as primary for the currently logged in user
    /// </summary>
    /// <param name="id">The ID of the address</param>
    /// <returns>
    /// <see cref="StatusCodes.Status200OK"/> If the address was successfully set as primary or if the address was already
    /// <see cref="StatusCodes.Status400BadRequest"/> If the id is 0 or less, if the update failed or if an exception was thrown
    /// <see cref="StatusCodes.Status401Unauthorized"/> If the user is not logged in or if the user is trying to set another
    /// user's address as primary
    /// <see cref="StatusCodes.Status404NotFound"/> If there is no address with the given ID
    /// </returns>
    [HttpPost("SetPrimary/{id}")]
    public IActionResult SetPrimary(int id)
    {
        if (id <= 0)
        {
            return BadRequest($"The provided address id is invalid, please provide a correct value");
        }

        try
        {
            string? currentUserId = _userManager.GetCurrentUserId;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized($"You must be logged in to set an address as primary");
            }

            AddressEntity? addressEntity = _addressRepository.GetByKey(id);

            if (addressEntity is null)
            {
                return NotFound($"Could not find any address with id : \"{id}\"");
            }

            if (!addressEntity.UserId.Equals(currentUserId))
            {
                return Unauthorized($"The currently logged in user is not allowed to set another user's address as primary");
            }

            if (addressEntity.IsPrimary)
            {
                return Ok($"The address with id : \"{id}\" is already the primary address");
            }

            var addressesFromRepo = _addressRepository.GetAll().Where(a => a.UserId.Equals(currentUserId));

            foreach (var addressFromRepo in addressesFromRepo)
            {
                if(addressFromRepo.Id != id)
                {
                    addressFromRepo.IsPrimary = false;
                    bool updateAddressFromRepoResult = _addressRepository.Update(addressFromRepo.Id, addressFromRepo);
                    if (!updateAddressFromRepoResult)
                    {
                        return BadRequest($"The address with id : \"{addressFromRepo.Id}\" couldn't be set as not primary");
                    }
                }
            }

            addressEntity.IsPrimary = true;

            bool updateSuccess = _addressRepository.Update(id, addressEntity);

            if (!updateSuccess)
            {
                return BadRequest($"The address with id : \"{id}\" couldn't be set as primary");
            }

            return Ok($"The address with id : \"{id}\" was successfully set as primary");
        }
        catch (Exception e)
        {
            _logger.LogError($"{DateTime.Now} - An exception was thrown during \"{nameof(SetPrimary)}\" :\n" +
                $"\"{e.Message}\"\n\"{e.StackTrace}\"");

#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest("An error occurred while trying to set the primary address, please contact the administrator");
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
    /// <see cref="StatusCodes.Status400BadRequest"/> If the provided ID or form are invalid or if another primary address
    /// exist and it IsPrimary property couldn't be changed to false or if the address couldn't be updated or if an 
    /// error occurred while trying to performe the update.
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

            if (addressToUpdate.IsPrimary)
            {
                AddressEntity? primaryAddressFromRepo = _addressRepository
                    .GetAll()
                    .Where(a => a.UserId.Equals(currentUserId) && a.IsPrimary && a.Id != id)
                    .SingleOrDefault();

                if (primaryAddressFromRepo is not null)
                {
                    primaryAddressFromRepo.IsPrimary = false;

                    if (!_addressRepository.Update(primaryAddressFromRepo.Id, primaryAddressFromRepo))
                    {
                        _logger.LogError($"{DateTime.Now} - An error occurred while trying to set the \"IsPrimary\" " +
                            $"property of the address \"{primaryAddressFromRepo.Id}\" to false");

                        return BadRequest($"An error occurred while trying to create the address. The new address " +
                            $"cannot be set to primary address");
                    }

                    _logger.LogInformation($"{DateTime.Now} - The old primary address has successfully been set " +
                        $"to false");
                }
            }

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

    /// <summary>
    /// Delete an address with the given ID
    /// </summary>
    /// <param name="id">The ID of the address to delete. Must be a positive number</param>
    /// <returns>
    /// <see cref="StatusCodes.Status200OK"/> If the delete went successfully
    /// <see cref="StatusCodes.Status400BadRequest"/> If the ID is a negative number or if the delete failed
    /// <see cref="StatusCodes.Status401Unauthorized"/> If the user is not logged in or if the user requesting the 
    /// delete is trying to delete another user's address and is not direction role
    /// <see cref="StatusCodes.Status404NotFound"/> If there is no address with the given ID
    /// </returns>
    [HttpDelete("{id}")]
    public IActionResult DeleteAddress(int id)
    {
        if (id < 0)
        {
            return BadRequest($"The given ID is a negative value");
        }

        try
        {
            string? currentUserId = _userManager.GetCurrentUserId;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized($"The user requesting the delete must be logged in");
            }

            AddressEntity? addressFromRepo = _addressRepository.GetByKey(id);

            if (addressFromRepo is null)
            {
                return NotFound($"There is no address with the given ID \"{id}\"");
            }

            if (!_userManager.IsDirection)
            {
                if (!addressFromRepo.UserId.Equals(currentUserId))
                {
                    return Unauthorized($"The currently logged in user is not allowed to delete another user's " +
                        $"address");
                }
            }

            if (!_addressRepository.Delete(id))
            {
                return BadRequest($"An error occurred while trying to delete the address. If failed");
            }

            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError($"{DateTime.Now} - An exception was thrown during \"{nameof(DeleteAddress)}\" :\n" +
                $"\"{e.Message}\"\n\"{e.StackTrace}\"");

#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest("An error occurred while trying to delete an address, please contact the administrator");
#endif
        }
    }
}
