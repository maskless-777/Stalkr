using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stalkr.Models;
using Stalkr.Repositories;

namespace Stalkr.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassesController : ControllerBase
    {
        private readonly ClassesRepository _repo;

        public ClassesController(IRepository<ClassesModel> repo)
        {
            _repo = repo as ClassesRepository ?? throw new ArgumentException("Repository must be ClassesRepository");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            var item = await _repo.FindByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ClassesModel dto)
        {
            await _repo.InsertAsync(dto);
            return Created($"/api/Classes/{dto.CourseID}", dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] ClassesModel dto)
        {
            var existing = await _repo.FindByIdAsync(id);
            if (existing == null) return NotFound();

            await _repo.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            var existing = await _repo.FindByIdAsync(id);
            if (existing == null) return NotFound();

            await _repo.DeleteAsync(id);
            return NoContent();
        }
    }
}
