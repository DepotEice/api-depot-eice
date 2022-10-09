using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.UIL.Mapper;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using AutoMapper;
using DevHopTools.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace API.DepotEice.UIL.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ModulesController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IModuleRepository _moduleRepository;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IScheduleFileRepository _scheduleFileRepository;

    public ModulesController(
        IMapper mapper,
        IModuleRepository moduleRepository,
        IScheduleRepository scheduleRepository,
        IScheduleFileRepository scheduleFileRepository)
    {
        _mapper = mapper;
        _moduleRepository = moduleRepository;
        _scheduleRepository = scheduleRepository;
        _scheduleFileRepository = scheduleFileRepository;
    }

    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            IEnumerable<ModuleModel> modules = _moduleRepository.GetAll().Select(x => x.Map<ModuleModel>());

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
            ModuleModel module = _mapper.Map<ModuleModel>(_moduleRepository.GetByKey(id));

            if (module == null)
                return NotFound();

            return Ok(module);
        }
        catch (Exception e)
        {
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
            int moduleId = _moduleRepository.Create(form.Map<ModuleEntity>());

            if (moduleId <= 0)
                return BadRequest("The creation failed");

            ModuleModel? result = _mapper.Map<ModuleModel>(_moduleRepository.GetByKey(moduleId));

            if (result == null)
                return NotFound();

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
                return BadRequest();

            ModuleModel? module = _mapper.Map<ModuleModel>(_moduleRepository.GetByKey(id));

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
                return BadRequest();
            }

            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{mId}/Schedules")]
    public IActionResult GetSchedules(int mId)
    {
        try
        {
            IEnumerable<ScheduleModel> schedules = _scheduleFileRepository.GetAll().Select(x => _mapper.Map<ScheduleModel>(x));

            return Ok(schedules);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{mId}/Schedules/{sId}")]
    public IActionResult GetSchedule(int mId, int sId)
    {
        try
        {
            var entity = _scheduleRepository.GetByKey(sId);
            ScheduleModel schedule = _mapper.Map<ScheduleModel>(entity);

            return Ok(schedule);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("{mId}/Schedules/")]
    public IActionResult PostSchedule(int mId, [FromBody] ScheduleForm form)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            int scheduleId = _scheduleRepository.Create(_mapper.Map<ScheduleEntity>(form));
            var entity = _scheduleRepository.GetByKey(scheduleId);
            ScheduleModel schedule = entity.Map<ScheduleModel>();

            return Ok(schedule);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("{mId}/Schedules/{sId}")]
    public IActionResult PutSchedule(int mId, int sId, [FromBody] ScheduleForm form)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var entity = form.Map<ScheduleEntity>();
            bool result = _scheduleRepository.Update(sId, entity);

            if (!result)
                return BadRequest(nameof(result));

            entity = _scheduleRepository.GetByKey(sId);
            ScheduleModel? schedule = entity.Map<ScheduleModel>();

            return Ok(schedule);
        }
        catch (Exception e)
        {
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
                return BadRequest();

            return NoContent();
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
            IEnumerable<UserModel> students = _moduleRepository.GetModuleStudents(mId).Select(x => x.Map<UserModel>());
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
            bool result = _moduleRepository.StudentApply(sId, mId);

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
            bool result = _moduleRepository.StudentAcceptExempt(sId, mId, decision);

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
