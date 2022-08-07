﻿using API.DepotEice.BLL.IServices;
using Microsoft.AspNetCore.Mvc;

namespace API.DepotEice.UIL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpeningHoursController : ControllerBase
    {
        private readonly IOpeningHoursService _openingHoursService;

        public OpeningHoursController(IOpeningHoursService openingHoursService)
        {
            _openingHoursService = openingHoursService;
        }

        /// <summary>
        /// Retrieves the opening hours of the educational institution.
        /// </summary>
        /// <returns>OpeningHour items</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult Post([FromBody] string value)
        {
            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] string value)
        {
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return Ok();
        }
    }
}
