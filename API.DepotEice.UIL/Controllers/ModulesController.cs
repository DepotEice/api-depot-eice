using API.DepotEice.UIL.Models.Forms;
using Microsoft.AspNetCore.Mvc;

namespace API.DepotEice.UIL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModulesController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var test = new List<string>()
            {
                "Module1",
                "Module2",
                "Module3",
                "Module4",
            };

            return Ok(test);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult Post([FromBody] ModuleForm form)
        {
            return Ok();
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
    }
}
