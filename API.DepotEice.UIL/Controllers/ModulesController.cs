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

        private List<ModuleModel> _listModule { get; set; } = new List<ModuleModel>()
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

        [HttpGet]
        public IActionResult Get()
        {
            var modules = _moduleService.GetModules();

            return Ok(modules);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult Post([FromBody] ModuleForm form)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                ModuleModel? result = _moduleService.CreateModule(form.ToBll())?.ToUil();
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
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return Ok();
        }

        [HttpGet("{mId}/Schedules")]
        public IActionResult GetSchedules(int mId)
        {
            return Ok();
        }

        [HttpGet("{mId}/Schedules/{sId}")]
        public IActionResult GetSchedule(int mId, int sId)
        {
            return Ok();
        }

        [HttpPost("{mId}/Schedules/{sId}")]
        public IActionResult PostSchedule(int mId, int sId, [FromBody] ScheduleForm form)
        {
            return Ok();
        }

        [HttpPut("{mId}/Schedules/{sId}")]
        public IActionResult PutSchedule(int mId, int sId, [FromBody] ModuleForm form)
        {
            return Ok();
        }

        [HttpDelete("{mId}/Schedules/{sId}")]
        public IActionResult DeleteSchedule(int mId, int sId)
        {
            return Ok();
        }

        [HttpGet("{mId}/Schedules/{sId}/Files")]
        public IActionResult GetScheduleFiles(int mId, int sId)
        {
            return Ok();
        }

        [HttpPost("{mId}/Schedules/{sId}/Files")]
        public IActionResult PostScheduleFiles(int mId, int sId)
        {
            return Ok();
        }

        [HttpPut("{mId}/Schedules/{sId}/Files/{fId}")]
        public IActionResult PutScheduleFiles(int mId, int sId, int fId)
        {
            return Ok();
        }

        [HttpDelete("{mId}/Schedules/{sId}/Files/{fId}")]
        public IActionResult DeleteScheduleFiles(int mId, int sId, int fId)
        {
            return Ok();
        }

        [HttpPost("{mId}/Teachers/{tId}")]
        public IActionResult AssignTeacher(int mId, string tId)
        {
            return Ok();
        }

        [HttpDelete("{mId}/Teachers/{tId}")]
        public IActionResult DischargeTeacher(int mId, string tId)
        {
            return Ok();
        }

        [HttpPost("{mId}/Students/{sId}")]
        public IActionResult StudentApply(int mId, string sId)
        {
            return Ok();
        }

        [HttpPut("{mId}/Students/{sId}")]
        public IActionResult StudentAccept(int mId, string sId)
        {
            return Ok();
        }

        [HttpDelete("{mId}/Students/{sId}")]
        public IActionResult StudentExempt(int mId, string sId)
        {
            return Ok();
        }
    }
}
