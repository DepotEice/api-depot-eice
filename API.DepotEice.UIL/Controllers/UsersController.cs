using API.DepotEice.UIL.Models.Forms;
using Microsoft.AspNetCore.Mvc;

namespace API.DepotEice.UIL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult Post([FromBody] UserForm form)
        {
            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] UserForm form)
        {
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            return Ok();
        }

        [HttpGet("{uId}/Appointments")]
        public IActionResult GetAppointments(string uId)
        {
            return Ok();
        }

        [HttpPost("{uId}/Appointments")]
        public IActionResult PostAppointment(string uId, [FromBody] AppointmentForm form)
        {
            return Ok();
        }
    }
}
