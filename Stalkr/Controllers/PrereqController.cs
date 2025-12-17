using Microsoft.AspNetCore.Mvc;
using Stalkr.Models;
using Stalkr.Repositories;

namespace Stalkr.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrereqController : ControllerBase
    {
        private readonly PrereqRepository _repo;

        public PrereqController(PrereqRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _repo.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("to/{courseId}")]
        public async Task<IActionResult> GetByToCourse(string courseId)
        {
            var list = await _repo.GetByToCourseIdAsync(courseId);
            if (!list.Any())
                return NotFound($"No prerequisites found for course {courseId}");
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PrereqRelationshipModel rel)
        {
            var success = await _repo.CreateAsync(rel);
            return success ? Ok("Prerequisite relationship created") : StatusCode(500, "Failed to create prerequisite");
        }

        [HttpDelete("{fromCourseId}/{toCourseId}")]
        public async Task<IActionResult> Delete(string fromCourseId, string toCourseId)
        {
            var success = await _repo.DeleteAsync(fromCourseId, toCourseId);
            return success ? Ok("Prerequisite deleted") : NotFound("Prerequisite not found");
        }
    }
}
