using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stalkr.Models;
using Stalkr.Repositories;

namespace Stalkr.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolsController : ControllerBase
    {
        private readonly IRepository<SchoolsModel> _repo;

        public SchoolsController(IRepository<SchoolsModel> repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPeopleAsync()
        {
            try
            {
                var people = await _repo.GetAllAsync();
                return Ok(people);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var person = await _repo.FindByIdAsync(id);
                if (person == null)
                    return NotFound();
                return Ok(person);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePersonAsync([FromBody] SchoolsModel pplModel)
        {
            try
            {
                await _repo.InsertAsync(pplModel);
                return Ok("School created");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePersonAsync(int id)
        {
            try
            {
                if (await _repo.FindByIdAsync(id) == null)
                    return NotFound($"School with id {id} cannot be found");

                await _repo.DeleteAsync(id);
                return Ok("School Deleted!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePersonAsync(int id, [FromBody] SchoolsModel updatedPeople)
        {
            try
            {
                if (await _repo.FindByIdAsync(id) == null)
                    return NotFound($"School with id {id} cannot be found");

                await _repo.UpdateAsync(id, updatedPeople);
                return Ok("School Updated!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }
    }
}
