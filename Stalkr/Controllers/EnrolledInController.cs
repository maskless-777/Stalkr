using Microsoft.AspNetCore.Mvc;
using Stalkr.Models;
using Stalkr.Repositories;

namespace Stalkr.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrolledInController : ControllerBase
    {
        private readonly EnrolledInRepository _repo;

        public EnrolledInController(EnrolledInRepository repo)
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
                return NotFound($"Person {personId} is not enrolled in any classes");
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EnrolledInRelationshipModel rel)
        {
            var success = await _repo.CreateAsync(rel);
            return success ? Ok("Enrollment created") : StatusCode(500, "Failed to create enrollment");
        }

        [HttpDelete("{personId}/{courseId}")]
        public async Task<IActionResult> Delete(int personId, int courseId)
        {
            var success = await _repo.DeleteAsync(personId, courseId);
            return success ? Ok("Enrollment deleted") : NotFound("Enrollment not found");
        }
    }
}
