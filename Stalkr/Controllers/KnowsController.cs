using Microsoft.AspNetCore.Mvc;
using Stalkr.Models;
using Stalkr.Repositories;

namespace Stalkr.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KnowsController : ControllerBase
    {
        private readonly KnowsRepository _repo;

        public KnowsController(KnowsRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _repo.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{fromPersonId}")]
        public async Task<IActionResult> GetByFromPersonId(int fromPersonId)
        {
            var list = await _repo.GetByFromPersonIdAsync(fromPersonId);
            if (!list.Any())
                return NotFound($"No KNOWS relationships found for person {fromPersonId}");
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] KnowsRelationshipModel rel)
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
