using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.UIL.Data;
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

    public ModulesController(ILogger<ModulesController> logger, IMapper mapper, IModuleRepository moduleRepository,
        IScheduleRepository scheduleRepository, IScheduleFileRepository scheduleFileRepository,
        IRoleRepository roleRepository, IUserRepository userRepository)
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

        _logger = logger;
        _mapper = mapper;
        _moduleRepository = moduleRepository;
        _scheduleRepository = scheduleRepository;
        _scheduleFileRepository = scheduleFileRepository;
        _roleRepository = roleRepository;
        _userRepository = userRepository;
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
            IEnumerable<ScheduleEntity> schedulesFromRepo = _scheduleRepository.GetModuleSchedules(mId);

            IEnumerable<ScheduleModel> schedules = _mapper.Map<IEnumerable<ScheduleModel>>(schedulesFromRepo);

            return Ok(schedules);
        }
        catch (Exception e)
        {
            _logger.LogError($"{DateTime.Now} - An exception was thrown when trying to call {nameof(GetSchedules)} " +
                $"with Module ID {mId}\n{e.Message}\n{e.StackTrace}");

            return BadRequest(e.Message);
        }
    }

    [HttpGet("{mId}/Schedules/{sId}")]
    public IActionResult GetSchedule(int mId, int sId)
    {
        try
        {
            ScheduleEntity? scheduleFromRepo = _scheduleRepository
                .GetModuleSchedules(mId)
                .SingleOrDefault(s => s.Id == sId);

            if (scheduleFromRepo is null)
            {
                return NotFound($"There is no Schedule with ID \"{sId}\" for Module \"{mId}\"");
            }

            ScheduleModel schedule = _mapper.Map<ScheduleModel>(scheduleFromRepo);

            return Ok(schedule);
        }
        catch (Exception e)
        {
            _logger.LogError($"{DateTime.Now} - An exception was thrown when trying to call {nameof(GetSchedule)} " +
                $"with Module ID {mId} and Schedule ID \"{sId}\"\n{e.Message}\n{e.StackTrace}");

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

    [HttpGet("{mId}/Schedules/{sId}/Files")]
    public IActionResult GetScheduleFiles(int mId, int sId)
    {
        try
        {
            IEnumerable<ScheduleFileModel> scheduleFiles = _scheduleFileRepository
                .GetScheduleFiles(sId)
                .Select(x => x.Map<ScheduleFileModel>());

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
            return BadRequest(ModelState);

        try
        {
            ModuleModel? module = _moduleRepository.GetByKey(mId)?.Map<ModuleModel>();
            if (module == null)
                return NotFound("Le module n'existe pas !");

            ScheduleModel? schedule = _scheduleRepository.GetByKey(sId)?.Map<ScheduleModel>();
            if (schedule == null)
                return NotFound("Le schedule n'existe pas !");

            string imagePath = SaveImageAndGetPath(file);

            ScheduleFileEntity entity = new ScheduleFileEntity()
            {
                FilePath = imagePath,
                ScheduleId = sId
            };

            int scheduleFileId = _scheduleFileRepository.Create(entity);

            if (scheduleFileId <= 0)
                return BadRequest(nameof(scheduleFileId));

            ScheduleFileModel? scheduleFile = _scheduleFileRepository.GetByKey(scheduleFileId)?.Map<ScheduleFileModel>();

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
            ModuleModel? module = _moduleRepository.GetByKey(mId)?.Map<ModuleModel>();
            if (module == null)
                return NotFound("Le module n'existe pas !");

            ScheduleModel? schedule = _scheduleRepository.GetByKey(sId)?.Map<ScheduleModel>();
            if (schedule == null)
                return NotFound("Le schedule n'existe pas !");

            ScheduleFileModel? scheduleFile = _scheduleFileRepository.GetByKey(fId)?.Map<ScheduleFileModel>();
            if (scheduleFile == null)
                return NotFound("Aucune fichier existant !");

            string filePath = scheduleFile.FilePath;

            FileInfo fileInfo = new FileInfo(filePath);

            try
            {
                bool result = _scheduleFileRepository.Delete(fId);

                if (!result)
                    return BadRequest("Something went wrong ...");

                fileInfo.Delete();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return NoContent();
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
