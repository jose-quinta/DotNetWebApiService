using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Server.Entities;
using Server.Services.PersonServices;

namespace Server.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class PersonController : ControllerBase {
        private readonly IPersonService _service;

        public PersonController(IPersonService service) {
            _service = service;
        }
        [HttpGet]
        public async Task<ActionResult<List<Person>>> Get() {
            return Ok(await _service.Get());
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> GetById(int id) {
            return Ok(await _service.GetById(id));
        }
        [HttpPost]
        public async Task<ActionResult<List<Person>>> Post(PersonDto request) {
            return Ok(await _service.Post(request));
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<Person>> Put(int id, PersonDto request) {
            return Ok(await _service.Put(id, request));
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<Person>> Delete(int id) {
            return Ok(await _service.Delete(id));
        }
    }
}