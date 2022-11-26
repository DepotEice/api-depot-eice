using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.UIL.Data;
using API.DepotEice.UIL.Interfaces;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using AutoMapper;
using DevHopTools.Mappers;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

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

    public ModulesController(ILogger<ModulesController> logger, IMapper mapper, IModuleRepository moduleRepository,
        IScheduleRepository scheduleRepository, IScheduleFileRepository scheduleFileRepository,
        IRoleRepository roleRepository, IUserRepository userRepository, IUserManager userManager)
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

        _logger = logger;
        _mapper = mapper;
        _moduleRepository = moduleRepository;
        _scheduleRepository = scheduleRepository;
        _scheduleFileRepository = scheduleFileRepository;
        _roleRepository = roleRepository;
        _userRepository = userRepository;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            IEnumerable<ModuleEntity> modulesFromRepo = _moduleRepository.GetAll();

            var modules = _mapper.Map<IEnumerable<ModuleModel>>(modulesFromRepo);

            foreach (var module in modules)
            {
                var usersFromRepo = _moduleRepository.GetModuleUsers(module.Id);

                foreach (var user in usersFromRepo)
                {
                    var roles = _roleRepository.GetUserRoles(user.Id);

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
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        try
        {
            var moduleFromRepo = _moduleRepository.GetByKey(id);

            if (moduleFromRepo is null)
            {
                return NotFound($"There is no module with ID \"{id}\"");
            }

            ModuleModel module = _mapper.Map<ModuleModel>(moduleFromRepo);

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
        try
        {
            var moduleFromRepo = _moduleRepository.GetByKey(mId);

            if (moduleFromRepo is null)
            {
                return NotFound(nameof(mId));
            }

            ModuleModel module = _mapper.Map<ModuleModel>(moduleFromRepo);

            var moduleUsers = _moduleRepository.GetModuleUsers(mId, RolesData.TEACHER_ROLE);

            var teacher = moduleUsers.SingleOrDefault();

            if (teacher is not null)
            {
                module.TeacherId = teacher.Id;
            }

            return Ok(module);
        }
        catch (Exception e)
        {
            _logger.LogError($"{DateTime.Now} - An exception was thrown.\n{e.Message}\n{e.StackTrace}");
            return BadRequest();
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

    [HttpGet("Schedules")]
    public IActionResult GetSchedules()
    {
        try
        {
            IEnumerable<ScheduleEntity> schedulesFromRepo = _scheduleRepository.GetAll();

            IEnumerable<ScheduleModel> schedules = _mapper.Map<IEnumerable<ScheduleModel>>(schedulesFromRepo);

            return Ok(schedules);
        }
        catch (Exception e)
        {
            _logger.LogError($"{DateTime.Now} - An exception was thrown when trying to call {nameof(GetSchedules)}\n" +
                $"{e.Message}\n{e.StackTrace}");

            return BadRequest(e.Message);
        }
    }

    [HttpGet("{mId}/Schedules")]
    public IActionResult GetSchedules(int mId)
    {
        try
        {
            string? userId = _userManager.GetCurrentUserId;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            bool isDirection = _roleRepository.GetUserRoles(userId).Any(r => r.Name.Equals(RolesData.DIRECTION_ROLE));

            if (isDirection)
            {
                IEnumerable<ScheduleEntity> schedulesFromRepo = _scheduleRepository.GetModuleSchedules(mId);

                IEnumerable<ScheduleModel> schedules = _mapper.Map<IEnumerable<ScheduleModel>>(schedulesFromRepo);

                return Ok(schedules);
            }

            var userModules = _moduleRepository.GetUserModules(userId);

            List<ScheduleModel> schedulesList = new();

            foreach (var module in userModules)
            {
                var schedulesFromRepo = _scheduleRepository.GetModuleSchedules(module.Id);

                IEnumerable<ScheduleModel> schedulesMapped = _mapper.Map<IEnumerable<ScheduleModel>>(schedulesFromRepo);

                schedulesList.AddRange(schedulesMapped);
            }

            return Ok(schedulesList);
        }
        catch (Exception e)
        {
            _logger.LogError($"{DateTime.Now} - An exception was thrown when trying to call {nameof(GetSchedules)} " +
                $"with Module ID {mId}\n{e.Message}\n{e.StackTrace}");

            return BadRequest(e.Message);
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

    [HttpPut("{mId}/Schedules/{sId}")]
    public IActionResult PutSchedule(int mId, int sId, [FromBody] ScheduleForm form)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            ScheduleEntity scheduleToUpdate = _mapper.Map<ScheduleEntity>(form);
            scheduleToUpdate.ModuleId = mId;

            bool result = _scheduleRepository.Update(sId, scheduleToUpdate);

            if (!result)
            {
                return BadRequest($"The update of the Schedule with ID \"{sId}\" failed");
            }

            ScheduleEntity? scheduleFromRepo = _scheduleRepository.GetByKey(sId);

            if (scheduleFromRepo is null)
            {
                return NotFound($"The Schedule with ID \"{sId}\" could not be found");
            }

            ScheduleModel schedule = _mapper.Map<ScheduleModel>(scheduleFromRepo);

            return Ok(schedule);
        }
        catch (Exception e)
        {
            _logger.LogError($"An exception was thrown when trying to call {nameof(PutSchedule)} width Module " +
                $"ID \"{mId}\" and Schedule ID \"{sId}\"\n{e.Message}\n{e.StackTrace}");

            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{mId}/Schedules/{sId}")]
    public IActionResult DeleteSchedule(int mId, int sId)
    {
        try
        {
            bool result = _scheduleRepository.Delete(sId);

            if (!result)
            {
                return BadRequest($"The deletion of the Schedule with ID \"{sId}\" for Module with ID " +
                    $"\"{mId}\" Failed");
            }

            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("Schedules/{sId}/Files")]
    public IActionResult GetScheduleFiles(int sId)
    {
        try
        {
            IEnumerable<ScheduleFileEntity> scheduleFilesFromRepo = _scheduleFileRepository.GetScheduleFiles(sId);

            List<ScheduleFileModel> scheduleFiles = new();

            foreach (var scheduleFile in scheduleFilesFromRepo)
            {
                ScheduleFileModel scheduleFileModel = _mapper.Map<ScheduleFileModel>(scheduleFile);

                if (!System.IO.File.Exists(scheduleFileModel.FilePath))
                {
                    return NotFound("The file doesn't exist in the file system");
                }

                string fileName = Path.GetFileName(scheduleFileModel.FilePath);
                string fileExtension = Path.GetExtension(scheduleFileModel.FilePath);

                scheduleFileModel.FileName = fileName;
                scheduleFileModel.FileExtension = fileExtension;

                scheduleFiles.Add(scheduleFileModel);
            }

            return Ok(scheduleFiles);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("{mId}/Schedules/{sId}/Files")]
    public IActionResult PostScheduleFiles(int mId, int sId, [FromForm] ScheduleFileForm file)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            ModuleEntity? moduleFromRepo = _moduleRepository.GetByKey(mId);

            if (moduleFromRepo is null)
            {
                return NotFound();
            }

            ScheduleEntity? scheduleFromRepo = _scheduleRepository.GetByKey(sId);

            if (scheduleFromRepo is null)
            {
                return NotFound();
            }

            string filePath = SaveImageAndGetPath(file);

            ScheduleFileEntity entity = new ScheduleFileEntity()
            {
                FilePath = filePath,
                ScheduleId = sId
            };

            int scheduleFileId = _scheduleFileRepository.Create(entity);

            if (scheduleFileId <= 0)
            {
                System.IO.File.Delete(filePath);
                return BadRequest(nameof(scheduleFileId));
            }

            ScheduleFileEntity? scheduleFileFromRepo = _scheduleFileRepository.GetByKey(scheduleFileId);

            if (scheduleFileFromRepo is null)
            {
                return NotFound();
            }

            ScheduleFileModel? scheduleFile = _mapper.Map<ScheduleFileModel>(scheduleFileFromRepo);

            return Ok(scheduleFile);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{mId}/Schedules/{sId}/Files/{fId}")]
    public IActionResult DeleteScheduleFiles(int mId, int sId, int fId)
    {
        try
        {
            var moduleFromRepo = _moduleRepository.GetByKey(mId);

            if (moduleFromRepo is null)
            {
                return NotFound($"No module with ID \"{mId}\" was found");
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

            ScheduleFileModel scheduleFile = _mapper.Map<ScheduleFileModel>(scheduleFileFromRepo);

            string filePath = scheduleFileFromRepo.FilePath;

            FileInfo fileInfo = new FileInfo(filePath);

            try
            {
                bool result = _scheduleFileRepository.Delete(fId);

                if (!result)
                {
                    return BadRequest($"The deletion of the file with ID \"{scheduleFile.Id}\" failed");
                }

                fileInfo.Delete();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{mId}/Teachers/")]
    public IActionResult GetTeachers(int mId)
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
            bool result = _moduleRepository.AcceptUser(sId, mId, decision);

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
        string fileName = file.File.FileName;

        string uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;

        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files/", uniqueFileName);
        file.File.CopyTo(new FileStream(imagePath, FileMode.Create, FileAccess.Write));
        return imagePath;
    }
}
