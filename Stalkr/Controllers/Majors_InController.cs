using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stalkr.Models;
using Stalkr.Repositories;

namespace Stalkr.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Majors_InController : ControllerBase
    {
        private readonly Majors_InRepository _repo;

        public Majors_InController(Majors_InRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWithDetails()
        {
            var list = await _repo.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{fromPersonId}")]
        public async Task<IActionResult> GetByFromPersonId(int fromPersonId)
        {
            var list = await _repo.GetByFromPersonIdAsync(fromPersonId);
            if (!list.Any())
                return NotFound($"No MAJORS_IN relationships found for person {fromPersonId}");
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Majors_InModel rel)
        {
            var success = await _repo.CreateAsync(rel);
            return success ? Ok("Relationship created") : StatusCode(500, "Failed to create relationship");
        }

        [HttpDelete("{fromId}/{toId}")]
        public async Task<IActionResult> Delete(int fromId, int toId)
        {
            var success = await _repo.DeleteAsync(fromId, toId);
            return success ? Ok("Relationship deleted") : NotFound("Relationship not found");
        }
    }
}
