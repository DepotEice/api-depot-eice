using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.UIL.AuthorizationAttributes;
using API.DepotEice.UIL.Data;
using API.DepotEice.UIL.Interfaces;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DevHopTools.Mappers;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Cryptography;
using static API.DepotEice.UIL.Data.RolesData;
using static API.DepotEice.UIL.Data.Utils;

namespace API.DepotEice.UIL.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ModulesController : ControllerBase
{
    private readonly ILogger<ModulesController> _logger;
    private readonly IMapper _mapper;
    private readonly IModuleRepository _moduleRepository;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IScheduleFileRepository _scheduleFileRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserManager _userManager;
    private readonly IFileManager _fileManager;
    private readonly IFileRepository _fileRepository;

    public ModulesController(ILogger<ModulesController> logger, IMapper mapper, IModuleRepository moduleRepository,
        IScheduleRepository scheduleRepository, IScheduleFileRepository scheduleFileRepository,
        IRoleRepository roleRepository, IUserRepository userRepository, IUserManager userManager, IFileManager fileManager,
        IFileRepository fileRepository)
    {
        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        if (mapper is null)
        {
            throw new ArgumentNullException(nameof(mapper));
        }

        if (moduleRepository is null)
        {
            throw new ArgumentNullException(nameof(moduleRepository));
        }

        if (scheduleRepository is null)
        {
            throw new ArgumentNullException(nameof(scheduleRepository));
        }

        if (scheduleFileRepository is null)
        {
            throw new ArgumentNullException(nameof(scheduleFileRepository));
        }

        if (roleRepository is null)
        {
            throw new ArgumentNullException(nameof(roleRepository));
        }

        if (userRepository is null)
        {
            throw new ArgumentNullException(nameof(userRepository));
        }

        if (userManager is null)
        {
            throw new ArgumentNullException(nameof(userManager));
        }

        if (fileManager is null)
        {
            throw new ArgumentNullException(nameof(fileManager));
        }

        if (fileRepository is null)
        {
            throw new ArgumentNullException(nameof(fileRepository));
        }

        _logger = logger;
        _mapper = mapper;
        _moduleRepository = moduleRepository;
        _scheduleRepository = scheduleRepository;
        _scheduleFileRepository = scheduleFileRepository;
        _roleRepository = roleRepository;
        _userRepository = userRepository;
        _userManager = userManager;
        _fileManager = fileManager;
        _fileRepository = fileRepository;
    }

    /// <summary>
    /// Get all the modules
    /// </summary>
    /// <returns>
    /// <see cref="IEnumerable{T}"/> where T is <see cref="ModuleModel"/>
    /// </returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ModuleModel>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GetModules()
    {
        try
        {
            IEnumerable<ModuleEntity> modulesFromRepo = _moduleRepository.GetAll();

            IEnumerable<ModuleModel> modules = _mapper.Map<IEnumerable<ModuleModel>>(modulesFromRepo);

            foreach (ModuleModel module in modules)
            {
                IEnumerable<UserEntity> usersFromRepo = _moduleRepository.GetModuleUsers(module.Id);

                foreach (UserEntity user in usersFromRepo)
                {
                    IEnumerable<RoleEntity> roles = _roleRepository.GetUserRoles(user.Id);

                    if (roles.Any(r => r.Name.Equals(RolesData.TEACHER_ROLE)))
                    {
                        module.TeacherId = user.Id;
                        break;
                    }
                }
            }

            return Ok(modules);
        }
        catch (Exception e)
        {
            _logger.LogError(
                "{date} - An exception was thrown during \"{fnName}\":\n{e.Message}\"\n\"{e.StackTrace}\"",
                DateTime.Now,
                nameof(GetModules),
                e.Message,
                e.StackTrace
            );
#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest("An error occurred while trying to get modules, please contact the administrator");
#endif
        }
    }

    /// <summary>
    /// Get the module with the specified ID
    /// </summary>
    /// <param name="id">
    /// The id of the module
    /// </param>
    /// <returns>
    /// <see cref="ModuleModel"/>
    /// </returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModuleModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetModule(int id)
    {
        try
        {
            ModuleEntity? moduleFromRepo = _moduleRepository.GetByKey(id);

            if (moduleFromRepo is null)
            {
                return NotFound($"There is no module with ID \"{id}\"");
            }

            ModuleModel module = _mapper.Map<ModuleModel>(moduleFromRepo);

            IEnumerable<UserEntity> moduleUsers = _moduleRepository.GetModuleUsers(id, RolesData.TEACHER_ROLE);

            UserEntity? teacher = moduleUsers.SingleOrDefault();

            if (teacher is not null)
            {
                module.TeacherId = teacher.Id;
            }

            return Ok(module);
        }
        catch (Exception e)
        {
            _logger.LogError(
                "{date} - An exception was thrown during \"{fnName}\":\n{e.Message}\"\n\"{e.StackTrace}\"",
                DateTime.Now,
                nameof(GetModule),
                e.Message,
                e.StackTrace
            );
#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest("An error occurred while trying to get the module, please contact the administrator");
#endif
        }
    }

    [HttpGet("{mId}/HasRole/{roleName}")]
    public IActionResult HasRole(int mId, string roleName)
    {
        try
        {
            string? currentUserId = _userManager.GetCurrentUserId;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized();
            }

            var moduleUsers = _moduleRepository.GetModuleUsers(mId);

            var moduleCurrentUser = moduleUsers.SingleOrDefault(u => u.Id.Equals(currentUserId));

            if (moduleCurrentUser is null)
            {
                return Ok(false);
            }

            var userRoles = _roleRepository.GetUserRoles(moduleCurrentUser.Id);

            if (userRoles.Any(r => r.Name.Equals(roleName)))
            {
                return Ok(true);
            }

            return Ok(false);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("RequestingUsers")]
    public IActionResult GetUserRequestingModule()
    {
        try
        {
            var modulesFromRepo = _moduleRepository.GetAll();

            List<UserRequestingModuleModel> usersNotAccepted = new();

            foreach (var module in modulesFromRepo)
            {
                IEnumerable<UserEntity> notAcceptedUsers = _moduleRepository
                    .GetModuleUsers(module.Id, RolesData.STUDENT_ROLE, false);

                var users = _mapper.Map<IEnumerable<UserRequestingModuleModel>>(notAcceptedUsers);

                foreach (var user in users)
                {
                    user.ModuleId = module.Id;
                    user.ModuleName = module.Name;
                }

                usersNotAccepted.AddRange(users);
            }

            return Ok(usersNotAccepted);
        }
        catch (Exception e)
        {
            _logger.LogError($"{DateTime.Now} - An exception was thrown when trying to execute " +
                $"{nameof(GetUserRequestingModule)}.\n{e.Message}\n{e.StackTrace}");

            return BadRequest(e.Message);
        }
    }

    [HttpGet("{mId}/UserRequestStatus")]
    public IActionResult GetUserRequestStatus(int mId)
    {
        string? userId = _userManager.GetCurrentUserId;

        if (userId is null)
        {
            return BadRequest();
        }

        bool? userIsAccepted = _moduleRepository.GetUserModuleStatus(mId, userId);

        return Ok(userIsAccepted);
    }

    [HttpGet("{mId}/UserRequestStatus/{userId}")]
    public IActionResult GetUserRequestStatus(int mId, string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(nameof(userId));
        }

        bool? userIsAccepted = _moduleRepository.GetUserModuleStatus(mId, userId);

        return Ok(userIsAccepted);
    }

    [HttpPost("{mId}/RequestAcceptance")]
    public IActionResult RequestAcceptance(int mId)
    {
        if (mId <= 0)
        {
            return BadRequest("The module id is invalid");
        }

        try
        {
            string? userId = _userManager.GetCurrentUserId;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("You must be authenticated to perform this action");
            }

            ModuleEntity? moduleFromRepo = _moduleRepository.GetByKey(mId);

            if (moduleFromRepo is null)
            {
                return NotFound("There is no module with the specified id");
            }

            ModuleModel module = _mapper.Map<ModuleModel>(moduleFromRepo);

            IEnumerable<UserEntity> moduleUsers = _moduleRepository.GetModuleUsers(mId);

            if (moduleUsers.Any(mu => mu.Id.Equals(userId)))
            {
                return BadRequest("You are already requested this module");
            }

            UserEntity? userEntity = _userRepository.GetByKey(userId);

            if (userEntity is null)
            {
                return BadRequest("There is no user with the specified id");
            }

            if (userEntity.DeletedAt is not null)
            {
                return BadRequest("You can't request a module because your account is deleted");
            }

            if (_userManager.IsInRole(TEACHER_ROLE))
            {
                return BadRequest("A teacher cannot ask to join a module");
            }

            bool result = _moduleRepository.AddUserToModule(userId, mId);


            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError(
                "{date} - An exception was thrown during \"{fnName}\":\n{e.Message}\"\n\"{e.StackTrace}\"",
                DateTime.Now,
                nameof(RequestAcceptance),
                e.Message,
                e.StackTrace
            );
#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest("An error occurred while trying to add user to a module, please contact the administrator");
#endif
        }
    }

    [HttpPost]
    public IActionResult Post([FromBody] ModuleForm form)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var moduleToCreate = _mapper.Map<ModuleEntity>(form);

            int moduleId = _moduleRepository.Create(moduleToCreate);

            if (moduleId <= 0)
            {
                return BadRequest("Module was not created");
            }

            var moduleFromRepo = _moduleRepository.GetByKey(moduleId);

            if (moduleFromRepo is null)
            {
                return NotFound($"Module with ID \"{moduleId}\" doesn't exist");
            }

            if (!string.IsNullOrEmpty(form.TeacherId))
            {
                bool addUserResult = _moduleRepository.AddUserToModule(form.TeacherId, moduleFromRepo.Id);

                if (addUserResult)
                {
                    _logger.LogInformation($"{DateTime.Now} - User with ID \"{form.TeacherId}\" correctly added " +
                        $"to the module with ID \"{moduleFromRepo.Id}\"");
                }
                else
                {
                    _logger.LogError($"{DateTime.Now} - User with ID \"{form.TeacherId}\" couldn't be added to " +
                        $"module with ID \"{moduleFromRepo.Id}\"");
                }
            }

            ModuleModel result = _mapper.Map<ModuleModel>(moduleFromRepo);

            result.TeacherId = form.TeacherId;

            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody] ModuleForm form)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            ModuleEntity entity = form.Map<ModuleEntity>();

            entity.Id = id;

            bool result = _moduleRepository.Update(id, entity);

            if (!result)
            {
                return BadRequest($"The update of Module with ID \"{id}\" failed");
            }

            var moduleFromRepo = _moduleRepository.GetByKey(id);

            if (moduleFromRepo is null)
            {
                return NotFound($"Recently create module with ID \"{id}\" could not be found");
            }

            ModuleModel? module = _mapper.Map<ModuleModel>(moduleFromRepo);

            var moduleUsers = _moduleRepository.GetModuleUsers(id, RolesData.TEACHER_ROLE);

            var teacher = moduleUsers.SingleOrDefault();

            if (teacher is not null)
            {
                module.TeacherId = teacher.Id;
            }

            return Ok(module);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        try
        {
            var result = _moduleRepository.Delete(id);

            if (!result)
            {
                return BadRequest($"The deletion of Module with ID \"{id}\" failed");
            }

            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    /// Get the schedules. If the user is admin, retrieve all the schedules from the database, otherwise retrieve only
    /// the schedules of the currently logged in user.
    /// </summary>
    /// <param name="selectedDate">
    /// The date selected by the user. If null, all the schedules are loaded
    /// </param>
    /// <param name="range">
    /// The range of the schedules to load. If not specified and if <paramref name="selectedDate"/> is not null, the
    /// range is set to Day which is 0
    /// </param>
    /// <returns>
    /// <see cref="IEnumerable{T}"/> where T is <see cref="ScheduleModel"/>
    /// </returns>
    [HttpGet("Schedules")]
    [HasRoleAuthorize(RolesEnum.STUDENT)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ScheduleModel>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
    public IActionResult GetSchedules(DateTime? selectedDate = null, DateRange range = DateRange.Day)
    {
        try
        {
            bool isDirection = _userManager.IsInRole(DIRECTION_ROLE);

            IEnumerable<ScheduleModel> schedules;

            if (isDirection)
            {
                IEnumerable<ScheduleEntity> schedulesFromRepo = _scheduleRepository.GetAll();

                schedules = _mapper.Map<IEnumerable<ScheduleModel>>(schedulesFromRepo);
            }
            else
            {
                string? userId = _userManager.GetCurrentUserId;

                if (userId is null)
                {
                    return Unauthorized("You must be authenticated to perform this action");
                }

                IEnumerable<ModuleEntity> modulesFromRepo = _moduleRepository.GetUserModules(userId);

                List<ScheduleEntity> userSchedules = new();

                foreach (ModuleEntity module in modulesFromRepo)
                {
                    IEnumerable<ScheduleEntity> schedulesFromRepo = _scheduleRepository.GetModuleSchedules(module.Id);

                    userSchedules.AddRange(schedulesFromRepo);
                }

                schedules = _mapper.Map<IEnumerable<ScheduleModel>>(userSchedules);
            }

            if (selectedDate.HasValue)
            {
                DateTime givenDate = selectedDate.Value;

                switch (range)
                {
                    case DateRange.Day:
                        schedules = schedules
                            .Where(a =>
                                a.StartAt.Year == selectedDate.Value.Year &&
                                a.StartAt.Month == selectedDate.Value.Month &&
                                a.StartAt.Day == selectedDate.Value.Day)
                            .ToList();
                        break;
                    case DateRange.Week:
                        DateTime startOfWeek = givenDate.AddDays(-(int)givenDate.DayOfWeek);
                        DateTime endOfWeek = startOfWeek.AddDays(7);

                        schedules = schedules
                            .Where(a => a.StartAt >= startOfWeek && a.StartAt <= endOfWeek)
                            .ToList();
                        break;
                    case DateRange.Month:

                        DateTime endOfMonth = givenDate.AddDays(35);

                        schedules = schedules
                            .Where(a =>
                                a.StartAt.Year == selectedDate.Value.Year &&
                                a.StartAt.Month >= givenDate.Month && a.StartAt.Month <= endOfMonth.Month)
                            .ToList();
                        break;
                    case DateRange.Year:
                        schedules = schedules
                            .Where(a => a.StartAt.Year == selectedDate.Value.Year)
                            .ToList();
                        break;
                    default:
                        _logger.LogError("The date range is not valid");
                        break;
                }
            }

            return Ok(schedules);
        }
        catch (Exception e)
        {
            _logger.LogError(
                "{date} - An exception was thrown during \"{fnName}\":\n{e.Message}\"\n\"{e.StackTrace}\"",
                DateTime.Now,
                nameof(GetSchedules),
                e.Message,
                e.StackTrace
            );
#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest("An error occurred while trying to get the schedules, please contact the administrator");
#endif
        }
    }

    /// <summary>
    /// Get the schedules for a module
    /// </summary>
    /// <param name="mId">
    /// The id of the module
    /// </param>
    /// <param name="selectedDate">
    /// The date for which the schedules are requested
    /// </param>
    /// <param name="range">
    /// The date range for which the schedules are requested
    /// </param>
    /// <returns>
    /// <see cref="IEnumerable{T}"/> where T is <see cref="ScheduleModel"/>
    /// </returns>
    [HttpGet("{mId}/Schedules")]
    [HasRoleAuthorize(RolesEnum.STUDENT, andAbove: true)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ScheduleModel>))]
    public IActionResult GetSchedules(int mId, DateTime? selectedDate = null, DateRange range = DateRange.Day)
    {
        if (mId <= 0)
        {
            return BadRequest($"Invalid module id {mId}");
        }

        try
        {
            string? userId = _userManager.GetCurrentUserId;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("You must be authenticated to perform this action");
            }

            IEnumerable<ScheduleEntity> schedulesFromRepo = _scheduleRepository.GetModuleSchedules(mId);

            var schedules = _mapper.Map<IEnumerable<ScheduleModel>>(schedulesFromRepo).ToList();

            if (selectedDate.HasValue)
            {
                DateTime givenDate = selectedDate.Value;

                switch (range)
                {
                    case DateRange.Day:
                        schedules = schedules
                            .Where(a =>
                                a.StartAt.Year == selectedDate.Value.Year &&
                                a.StartAt.Month == selectedDate.Value.Month &&
                                a.StartAt.Day == selectedDate.Value.Day)
                            .ToList();
                        break;
                    case DateRange.Week:
                        DateTime startOfWeek = givenDate.AddDays(-(int)givenDate.DayOfWeek);
                        DateTime endOfWeek = startOfWeek.AddDays(7);

                        schedules = schedules
                            .Where(a => a.StartAt >= startOfWeek && a.StartAt <= endOfWeek)
                            .ToList();
                        break;
                    case DateRange.Month:

                        DateTime endOfMonth = givenDate.AddDays(35);

                        schedules = schedules
                            .Where(a =>
                                a.StartAt.Year == selectedDate.Value.Year &&
                                a.StartAt.Month >= givenDate.Month && a.StartAt.Month <= endOfMonth.Month)
                            .ToList();
                        break;
                    case DateRange.Year:
                        schedules = schedules
                            .Where(a => a.StartAt.Year == selectedDate.Value.Year)
                            .ToList();
                        break;
                    default:
                        _logger.LogError("The date range is not valid");
                        break;
                }
            }

            return Ok(schedules);
        }
        catch (Exception e)
        {
            _logger.LogError(
                "{date} - An exception was thrown during \"{fnName}\":\n{e.Message}\"\n\"{e.StackTrace}\"",
                DateTime.Now,
                nameof(GetSchedules),
                e.Message,
                e.StackTrace
            );
#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest("An error occurred while trying to get module's schedules, please contact the administrator");
#endif
        }
    }

    [HttpGet("Schedules/{sId}")]
    public IActionResult GetSchedule(int sId)
    {
        try
        {
            ScheduleEntity? scheduleFromRepo = _scheduleRepository.GetByKey(sId);

            if (scheduleFromRepo is null)
            {
                return NotFound($"There is no Schedule with ID \"{sId}\"");
            }

            ScheduleModel schedule = _mapper.Map<ScheduleModel>(scheduleFromRepo);

            return Ok(schedule);
        }
        catch (Exception e)
        {
            _logger.LogError($"{DateTime.Now} - An exception was thrown when trying to call {nameof(GetSchedule)} " +
                $"and Schedule ID \"{sId}\"\n{e.Message}\n{e.StackTrace}");

            return BadRequest(e.Message);
        }
    }

    [HttpPost("{mId}/Schedules/")]
    public IActionResult PostSchedule(int mId, [FromBody] ScheduleForm form)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            ScheduleEntity scheduleToCreate = _mapper.Map<ScheduleEntity>(form);
            scheduleToCreate.ModuleId = mId;

            int scheduleId = _scheduleRepository.Create(scheduleToCreate);

            ScheduleEntity? scheduleFromRepo = _scheduleRepository.GetByKey(scheduleId);

            if (scheduleFromRepo is null)
            {
                return NotFound($"The recently create Schedule could not be found");
            }

            ScheduleModel schedule = _mapper.Map<ScheduleModel>(scheduleFromRepo);

            return Ok(schedule);
        }
        catch (Exception e)
        {
            _logger.LogError($"{DateTime.Now} - An exception was thrown when trying to call {nameof(PostSchedule)}\n" +
                $"{e.Message}\n{e.StackTrace}");

            return BadRequest(e.Message);
        }
    }

    /// <summary>
    /// Update a module's schedule
    /// </summary>
    /// <param name="mId">
    /// The id of the module to which the schedule belongs
    /// </param>
    /// <param name="sId">
    /// The id of the schedule
    /// </param>
    /// <param name="form">
    /// The form containing the new schedule data
    /// </param>
    /// <returns>
    /// The newly created schedule
    /// </returns>
    [HttpPut("{mId}/Schedules/{sId}")]
    [HasRoleAuthorize(RolesEnum.TEACHER, andAbove: false)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ScheduleModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult PutSchedule(int mId, int sId, [FromBody] ScheduleForm form)
    {
        if (mId <= 0)
        {
            return BadRequest($"Invalid module id {mId}");
        }

        if (sId <= 0)
        {
            return BadRequest($"Invalid schedule id {sId}");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            string? userId = _userManager.GetCurrentUserId;

            if (userId is null)
            {
                return Unauthorized("You must be authenticated to perform this action");
            }

            ModuleEntity? moduleFromRepo = _moduleRepository.GetByKey(mId);

            if (moduleFromRepo is null)
            {
                return NotFound($"There is no Module with ID \"{mId}\"");
            }

            IEnumerable<UserEntity> moduleUsers = _moduleRepository.GetModuleUsers(mId);

            if (!moduleUsers.Any(u => u.Id.Equals(userId)))
            {
                return Unauthorized("You are not allowed to perform this action");
            }

            ScheduleEntity? scheduleFromRepo = _scheduleRepository.GetByKey(sId);

            if (scheduleFromRepo is null)
            {
                return NotFound($"There is no Schedule with ID \"{sId}\"");
            }

            ScheduleEntity scheduleToUpdate = _mapper.Map<ScheduleEntity>(form);

            scheduleToUpdate.ModuleId = mId;

            bool result = _scheduleRepository.Update(sId, scheduleToUpdate);

            if (!result)
            {
                return BadRequest($"The update of the Schedule with ID \"{sId}\" failed");
            }

            scheduleFromRepo = _scheduleRepository.GetByKey(sId);

            if (scheduleFromRepo is null)
            {
                return NotFound($"The updated schedule with ID \"{sId}\" could not be found");
            }

            ScheduleModel schedule = _mapper.Map<ScheduleModel>(scheduleFromRepo);

            return Ok(schedule);
        }
        catch (Exception e)
        {
            _logger.LogError(
                "{date} - An exception was thrown during \"{fnName}\":\n{e.Message}\"\n\"{e.StackTrace}\"",
                DateTime.Now,
                nameof(PutSchedule),
                e.Message,
                e.StackTrace
            );
#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest("An error occurred while trying to update a schedule, please contact the administrator");
#endif
        }
    }

    /// <summary>
    /// Delete a schedule by its ID from a module by its ID
    /// </summary>
    /// <param name="mId">The id of the module</param>
    /// <param name="sId">The id of the schedule</param>
    /// <returns></returns>
    [HttpDelete("{mId}/Schedules/{sId}")]
    public IActionResult DeleteSchedule(int mId, int sId)
    {
        if (mId <= 0 || sId <= 0)
        {
            return BadRequest("The Module ID and Schedule ID must be greater than 0");
        }

        try
        {
            ModuleEntity? moduleFromRepo = _moduleRepository.GetByKey(mId);

            if (moduleFromRepo is null)
            {
                return NotFound($"There is no Module with ID \"{mId}\"");
            }

            ScheduleEntity? scheduleFromRepo = _scheduleRepository.GetByKey(sId);

            if (scheduleFromRepo is null)
            {
                return NotFound($"There is no Schedule with ID \"{sId}\"");
            }

            bool result = _scheduleRepository.Delete(sId);

            if (!result)
            {
                return BadRequest($"The deletion of the Schedule with ID \"{sId}\" for Module with ID " +
                    $"\"{mId}\" Failed");
            }

            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError(
                "{date} - An exception was thrown during \"{fnName}\":\n{e.Message}\"\n\"{e.StackTrace}\"",
                DateTime.Now,
                nameof(DeleteSchedule),
                e.Message,
                e.StackTrace
            );
#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest("An error occurred while trying to delete a schedule, please contact the administrator");
#endif
        }
    }

    /// <summary>
    /// Get all the files of a schedule
    /// </summary>
    /// <param name="mId">
    /// The id of the module to which the schedule belongs
    /// </param>
    /// <param name="sId">
    /// The id of the schedule to which the files belong
    /// </param>
    /// <returns>
    /// List of files of the schedule
    /// </returns>
    [HttpGet("{mId}/Schedules/{sId}/Files")]
    [HasRoleAuthorize(RolesEnum.STUDENT)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ScheduleFileModel>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetScheduleFiles(int mId, int sId)
    {
        if (mId <= 0)
        {
            return BadRequest($"Invalid module id {mId}");
        }

        if (sId <= 0)
        {
            return BadRequest($"Invalid schedule id {sId}");
        }

        try
        {
            ModuleEntity? moduleFromRepo = _moduleRepository.GetByKey(mId);

            if (moduleFromRepo is null)
            {
                return NotFound($"There is no Module with ID \"{mId}\"");
            }

            ScheduleEntity? scheduleFromRepo = _scheduleRepository.GetByKey(sId);

            if (scheduleFromRepo is null)
            {
                return NotFound($"There is no Schedule with ID \"{sId}\"");
            }

            IEnumerable<ScheduleFileEntity> scheduleFilesFromRepo = _scheduleFileRepository.GetScheduleFiles(sId);

            List<ScheduleFileModel> scheduleFiles = new();

            foreach (ScheduleFileEntity scheduleFile in scheduleFilesFromRepo)
            {
                FileEntity? fileFromRepo = _fileRepository.GetByKey(scheduleFile.FileId);

                if (fileFromRepo is null)
                {
                    _logger.LogError(
                        "{date} - The file with ID \"{fileId}\" could not be found",
                        DateTime.Now,
                        scheduleFile.FileId
                    );

                    continue;
                }

                ScheduleFileModel scheduleFileModel = new ScheduleFileModel()
                {
                    Id = scheduleFile.Id,
                    FileName = fileFromRepo.Key,
                    FileId = fileFromRepo.Id,
                    ScheduleId = sId
                };

                scheduleFiles.Add(scheduleFileModel);
            }

            return Ok(scheduleFiles);
        }
        catch (Exception e)
        {
            _logger.LogError(
                "{date} - An exception was thrown during \"{fnName}\":\n{e.Message}\"\n\"{e.StackTrace}\"",
                DateTime.Now,
                nameof(GetScheduleFiles),
                e.Message,
                e.StackTrace
            );
#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest("An error occurred while trying to get schedule files, please contact the administrator");
#endif
        }
    }

    /// <summary>
    /// Save files to a schedule
    /// </summary>
    /// <param name="mId">
    /// The id of the module to which the schedule belongs
    /// </param>
    /// <param name="sId">
    /// The id of the schedule
    /// </param>
    /// <param name="files">
    /// The list of files to save
    /// </param>
    /// <returns></returns>
    [HttpPost("{mId}/Schedules/{sId}/Files")]
    [HasRoleAuthorize(RolesEnum.TEACHER, andAbove: false)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PostScheduleFiles(int mId, int sId, [FromForm] IEnumerable<IFormFile> files)
    {
        if (mId <= 0)
        {
            return BadRequest($"Invalid module id {mId}");
        }

        if (sId <= 0)
        {
            return BadRequest($"Invalid schedule id {sId}");
        }

        if (files is null || files.Count() <= 0)
        {
            return BadRequest("No files were provided");
        }

        try
        {
            ModuleEntity? moduleFromRepo = _moduleRepository.GetByKey(mId);

            if (moduleFromRepo is null)
            {
                return NotFound($"There is no Module with ID \"{mId}\"");
            }

            string? userId = _userManager.GetCurrentUserId;

            if (userId is null)
            {
                return Unauthorized("You must be authenticated to perform this action");
            }

            IEnumerable<UserEntity> moduleUsers = _moduleRepository.GetModuleUsers(mId);

            if (!moduleUsers.Any(u => u.Id.Equals(userId)))
            {
                return Unauthorized("You are not allowed to perform this action");
            }

            ScheduleEntity? scheduleFromRepo = _scheduleRepository.GetByKey(sId);

            if (scheduleFromRepo is null)
            {
                return NotFound($"There is no Schedule with ID \"{sId}\"");
            }

            foreach (IFormFile file in files)
            {
                string fileName = $"{scheduleFromRepo.StartAt:s}-{file.FileName}";

                if (!await _fileManager.UploadObjectAsync(file, fileName))
                {
                    BadRequest($"The file {file.FileName} could not be uploaded");
                }

                FileEntity fileEntity = new()
                {
                    Key = fileName,
                    Type = file.ContentType,
                    Size = file.Length
                };

                int createdFileId = _fileRepository.Create(fileEntity);

                if (createdFileId <= 0)
                {
                    BadRequest($"The file {file.FileName} could not be saved");
                }

                ScheduleFileEntity scheduleFileEntity = new ScheduleFileEntity
                {
                    ScheduleId = sId,
                    FileId = createdFileId
                };

                int createdScheduleFileId = _scheduleFileRepository.Create(scheduleFileEntity);

                if (createdScheduleFileId <= 0)
                {
                    BadRequest($"The file {file.FileName} could not be linked to the schedule");
                }
            }

            return NoContent();
        }
        catch (Exception e)
        {
            _logger.LogError(
                "{date} - An exception was thrown during \"{fnName}\":\n{e.Message}\"\n\"{e.StackTrace}\"",
                DateTime.Now,
                nameof(PostScheduleFiles),
                e.Message,
                e.StackTrace
            );
#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest("An error occurred while trying to update a schedule, please contact the administrator");
#endif
        }
    }

    /// <summary>
    /// Delete a file from a schedule
    /// </summary>
    /// <param name="mId">
    /// The id of the module to which the schedule belongs
    /// </param>
    /// <param name="sId">
    /// The id of the schedule to which the file belongs
    /// </param>
    /// <param name="fId">
    /// The id of the file to delete
    /// </param>
    /// <returns>
    /// true If the file was deleted successfully. false Otherwise
    /// </returns>
    [HttpDelete("{mId}/Schedules/{sId}/Files/{fId}")]
    [HasRoleAuthorize(RolesEnum.TEACHER, andAbove: false)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteScheduleFiles(int mId, int sId, int fId)
    {
        if (mId <= 0)
        {
            return BadRequest($"Invalid module id {mId}");
        }

        if (sId <= 0)
        {
            return BadRequest($"Invalid schedule id {sId}");
        }

        if (fId <= 0)
        {
            return BadRequest($"Invalid file id {fId}");
        }

        try
        {
            ModuleEntity? moduleFromRepo = _moduleRepository.GetByKey(mId);

            if (moduleFromRepo is null)
            {
                return NotFound($"No module with ID \"{mId}\" was found");
            }

            string? userId = _userManager.GetCurrentUserId;

            if (userId is null)
            {
                return Unauthorized("You must be authenticated to perform this action");
            }

            IEnumerable<UserEntity> moduleUsers = _moduleRepository.GetModuleUsers(mId);

            if (!moduleUsers.Any(u => u.Id.Equals(userId)))
            {
                return Unauthorized("You are not allowed to perform this action");
            }

            ScheduleEntity? scheduleFromRepo = _scheduleRepository.GetByKey(sId);

            if (scheduleFromRepo is null)
            {
                return NotFound($"No Schedule with ID \"{sId}\" was found");
            }

            ScheduleFileEntity? scheduleFileFromRepo = _scheduleFileRepository.GetByKey(fId);

            if (scheduleFileFromRepo is null)
            {
                return NotFound($"No Schedule File with ID \"{fId}\" was found");
            }

            FileEntity? fileFromRepo = _fileRepository.GetByKey(scheduleFileFromRepo.FileId);

            if (fileFromRepo is null)
            {
                return NotFound($"No File with ID \"{scheduleFileFromRepo.FileId}\" was found");
            }

            bool awsResult = await _fileManager.DeleteObjectAsync(fileFromRepo.Key);

            if (!awsResult)
            {
                return BadRequest($"The file {fileFromRepo.Key} could not be deleted");
            }

            bool fileResult = _fileRepository.Delete(fileFromRepo.Id);

            if (!fileResult)
            {
                return BadRequest($"The deletion of the file with ID \"{fileFromRepo.Id}\" failed");
            }

            bool scheduleFileResult = _scheduleFileRepository.Delete(fId);

            if (!scheduleFileResult)
            {
                return BadRequest($"The deletion of the file with ID \"{scheduleFileFromRepo.Id}\" failed");
            }

            return Ok(awsResult && fileResult && scheduleFileResult);
        }
        catch (Exception e)
        {
            _logger.LogError(
                "{date} - An exception was thrown during \"{fnName}\":\n{e.Message}\"\n\"{e.StackTrace}\"",
                DateTime.Now,
                nameof(DeleteScheduleFiles),
                e.Message,
                e.StackTrace
            );
#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest("An error occurred while trying to get addresses, please contact the administrator");
#endif
        }
    }

    /// <summary>
    /// Get the teacher of the module
    /// </summary>
    /// <param name="mId">The id of the module from which we want to retrieve the teacher</param>
    /// <returns>
    /// <see cref="UserModel"/> The teacher of the module
    /// </returns>
    [HttpGet("{mId}/Teacher/")]
    public IActionResult GetTeacher(int mId)
    {
        if (mId <= 0)
        {
            return BadRequest("Invalid module id");
        }

        try
        {
            ModuleEntity? moduleFromRepo = _moduleRepository.GetByKey(mId);

            if (moduleFromRepo is null)
            {
                return NotFound($"There is no module with ID {mId}");
            }

            IEnumerable<UserEntity> usersFromRepo = _moduleRepository.GetModuleUsers(mId);

            foreach (UserEntity user in usersFromRepo)
            {
                bool isTeacher = _roleRepository.GetUserRoles(user.Id).Any(x => x.Name.Equals(RolesData.TEACHER_ROLE));

                if (isTeacher)
                {
                    UserModel userModel = _mapper.Map<UserModel>(user);

                    return Ok(userModel);
                }
            }

            return NotFound($"The teacher of the module {mId} could not be found");
        }
        catch (Exception e)
        {
            _logger.LogError(
                "{date} - An exception was thrown during \"{fnName}\":\n{e.Message}\"\n\"{e.StackTrace}\"",
                DateTime.Now,
                nameof(GetTeacher),
                e.Message,
                e.StackTrace
            );
#if DEBUG
            return BadRequest(e.Message);
#else
            return BadRequest("An error occurred while trying to get the teacher, please contact the administrator");
#endif
        }
    }

    [HttpPost("{mId}/Teachers/{tId}")]
    public IActionResult AssignTeacher(int mId, string tId)
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

    [HttpDelete("{mId}/Teachers/{tId}")]
    public IActionResult DischargeTeacher(int mId, string tId)
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

    [HttpGet("{mId}/Students")]
    public IActionResult ModuleStudents(int mId)
    {
        try
        {
            IEnumerable<UserModel> students = _moduleRepository.GetModuleUsers(mId).Select(x => x.Map<UserModel>());
            return Ok(students);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("{mId}/Students/{sId}")]
    public IActionResult StudentApply(int mId, string sId)
    {
        try
        {
            bool result = _moduleRepository.AddUserToModule(sId, mId);

            if (!result)
                return BadRequest("Something went wrong ...");

            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("{mId}/Students/{sId}")]
    public IActionResult StudentAcceptExempt(int mId, string sId, bool decision)
    {
        try
        {
            // URL: .../Modules/{moduleId}/Students/{StudentId}?decision=false
            // cannot be accessible by guests or students. Only by Teacher or Higher
            bool result = _moduleRepository.SetUserStatus(sId, mId, decision);

            if (!result)
                return BadRequest("Something went wrong ...");

            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{mId}/Students/{sId}")]
    public IActionResult StudentDelete(int mId, string sId)
    {
        try
        {
            bool result = _moduleRepository.DeleteUserFromModule(sId, mId);

            if (!result)
                return BadRequest("Something went wrong ...");

            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    private static string SaveImageAndGetPath(ScheduleFileForm file)
    {
#if DEBUG
        string fileName = file.File.FileName;

        string uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;

        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files/", uniqueFileName);
        file.File.CopyTo(new FileStream(imagePath, FileMode.Create, FileAccess.Write));
        return imagePath;
#else
        Account account = new Account("dhea8umqv", "872675634599566", "RZlkP5LQs1WLXmueNw8iMlh8z_E");

        Cloudinary cloudinary = new Cloudinary(account);

        Stream stream = file.File.OpenReadStream();

        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(file.File.FileName, stream),
            PublicId = file.File.FileName
        };

        var uploadResult = cloudinary.Upload(uploadParams);

        return uploadResult.Url.ToString();
#endif
    }
}
