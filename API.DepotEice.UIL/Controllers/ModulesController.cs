using API.DepotEice.BLL.IServices;
using API.DepotEice.UIL.Mapper;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.DepotEice.UIL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModulesController : ControllerBase
    {
        private readonly IModuleService _moduleService;
        private readonly IScheduleService _scheduleService;
        private readonly IScheduleFileService _scheduleFileService;
        private readonly IMapper _mapper;

        private List<ModuleModel> FakeData { get; set; } = new List<ModuleModel>()
        {
            new ModuleModel()
            {
                Id = 1,
                Name = "Module 1",
                Description = "Description of module 1"
            },
            new ModuleModel()
            {
                Id = 2,
                Name = "Module 2",
                Description = "Description of module 2"
            },
        };

        public ModulesController(IModuleService moduleService, IScheduleService scheduleService, IScheduleFileService scheduleFileService, IMapper mapper)
        {
            _moduleService = moduleService;
            _scheduleService = scheduleService;
            _scheduleFileService = scheduleFileService;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves the list of modules.
        /// </summary>
        /// <returns>Returns a list of modules.</returns>
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                IEnumerable<ModuleModel> modules = _moduleService.GetAll().Select(x => x.ToUil());

                return Ok(modules);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Retrieves one item of module by it's id.
        /// </summary>
        /// <param name="id">The ID of the module.</param>
        /// <returns>returns the selected </returns>
        /// <response code="200">Returns selected item.</response>
        /// <response code="400">If the item is null.</response>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var item = _moduleService.GetByKey(id)?.ToUil();

                if (item == null)
                {
                    return NotFound();
                }

                return Ok(item);
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
                ModuleModel? result = _moduleService.Create(form.ToBll())?.ToUil();

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

                ModuleModel? module = _moduleService.Update(id, form.ToBll())?.ToUil();

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
                var result = _moduleService.Delete(id);

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
                var schedules = _scheduleService.GetModuleSchedules(mId).Select(x => _mapper.Map<ScheduleModel>(x));

                return !schedules.Any() ? NoContent() : Ok(schedules);
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
                ScheduleModel? item = _mapper.Map<ScheduleModel>(_scheduleService.GetSchedule(sId));

                return Ok(item);
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
                var response = _scheduleService.CreateSchedule(mId, _mapper.Map<BLL.Models.ScheduleData>(form));

                return Ok(response);
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
                ScheduleModel? item = _scheduleService.Update(sId, form.ToBll())?.ToUil();
                return Ok(item);
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
                var result = _scheduleService.Delete(sId);

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
                IEnumerable<ScheduleFileModel>? items = _scheduleFileService.GetScheduleFiles(sId).Select(x => x.ToUil());
                return Ok(items);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("{mId}/Schedules/{sId}/Files")]
        public IActionResult PostScheduleFiles(int mId, int sId, [FromForm]ScheduleFileForm file)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            try
            {
                var module = _moduleService.GetByKey(mId)?.ToUil();
                if (module == null)
                    return NotFound("Le module n'existe pas !");

                var schedule = _scheduleService.GetByKey(sId)?.ToUil();
                if (schedule == null)
                    return NotFound("Le schedule n'existe pas !");

                string imagePath = SaveImageAndGetPath(file);

                ScheduleFileModel? resposne = _scheduleFileService.CreateScheduleFile(sId, new ScheduleFileModel() { FilePath = imagePath }.ToBll())?.ToUil();

                return Ok(resposne);
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
                var module = _moduleService.GetByKey(mId)?.ToUil();
                if (module == null)
                    return NotFound("Le module n'existe pas !");

                var schedule = _scheduleService.GetByKey(sId)?.ToUil();
                if (schedule == null)
                    return NotFound("Le schedule n'existe pas !");

                var item = _scheduleFileService.GetScheduleFile(fId)?.ToUil();
                if (item == null)
                    return NotFound("Aucune fichier existant !");

                string filePath = item.FilePath;

                FileInfo fileInfo = new FileInfo(filePath);

                try
                {
                    var result = _scheduleFileService.DeleteScheduleFile(fId);

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

        [HttpPost("{mId}/Students/{sId}")]
        public IActionResult StudentApply(int mId, string sId)
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

        [HttpPut("{mId}/Students/{sId}")]
        public IActionResult StudentAccept(int mId, string sId)
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

        [HttpDelete("{mId}/Students/{sId}")]
        public IActionResult StudentExempt(int mId, string sId)
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

        private static string SaveImageAndGetPath(ScheduleFileForm file)
        {
            string fileName = file.File.FileName;
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;

            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files/", uniqueFileName);
            file.File.CopyTo(new FileStream(imagePath, FileMode.Create, FileAccess.Write));
            return imagePath;
        }
    }
}
