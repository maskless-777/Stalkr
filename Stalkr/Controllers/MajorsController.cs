using Microsoft.AspNetCore.Mvc;
using Stalkr.Models;
using Stalkr.Repositories;

namespace Stalkr.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MajorsController : ControllerBase
    {
        private readonly IRepository<MajorModel> _repo;

        public MajorsController(IRepository<MajorModel> repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMajors()
        {
            var majors = await _repo.GetAllAsync();
            return Ok(majors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMajor(int id)
        {
            var major = await _repo.FindByIdAsync(id);
            if (major == null) return NotFound();
            return Ok(major);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMajor([FromBody] MajorModel major)
        {
            await _repo.InsertAsync(major);
            return Ok("Major created");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMajor(int id, [FromBody] MajorModel major)
        {
            var existing = await _repo.FindByIdAsync(id);
            if (existing == null) return NotFound();
            await _repo.UpdateAsync(id, major);
            return Ok("Major updated");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMajor(int id)
        {
            var existing = await _repo.FindByIdAsync(id);
            if (existing == null) return NotFound();
            await _repo.DeleteAsync(id);
            return Ok("Major deleted");
        }
    }
}
