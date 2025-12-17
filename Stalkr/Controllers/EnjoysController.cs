using Microsoft.AspNetCore.Mvc;
using Stalkr.Models;
using Stalkr.Repositories;

namespace Stalkr.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnjoysController : ControllerBase
    {
        private readonly EnjoysRepository _repo;

        public EnjoysController(EnjoysRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _repo.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("person/{personId}")]
        public async Task<IActionResult> GetByPerson(int personId)
        {
            var list = await _repo.GetByPersonIdAsync(personId);
            if (!list.Any())
                return NotFound($"Person {personId} has no hobbies");
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EnjoysRelationshipModel rel)
        {
            var success = await _repo.CreateAsync(rel);
            return success ? Ok("Enjoys relationship created") : StatusCode(500, "Failed to create relationship");
        }

        [HttpDelete("{personId}/{hobbyId}")]
        public async Task<IActionResult> Delete(int personId, int hobbyId)
        {
            var success = await _repo.DeleteAsync(personId, hobbyId);
            return success ? Ok("Enjoys relationship deleted") : NotFound("Relationship not found");
        }
    }
}
