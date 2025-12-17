using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stalkr.Models;
using Stalkr.Repositories;

namespace Stalkr.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HobbyController : ControllerBase
    {
        private readonly IRepository<HobbyModel> _repo;

        public HobbyController(IRepository<HobbyModel> repo)
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
        public async Task<IActionResult> CreatePersonAsync([FromBody] HobbyModel pplModel)
        {
            try
            {
                await _repo.InsertAsync(pplModel);
                return Ok("Person created");
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
                    return NotFound($"Person with id {id} cannot be found");

                await _repo.DeleteAsync(id);
                return Ok("Person Deleted!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePersonAsync(int id, [FromBody] HobbyModel updatedPeople)
        {
            try
            {
                if (await _repo.FindByIdAsync(id) == null)
                    return NotFound($"Person with id {id} cannot be found");

                await _repo.UpdateAsync(id, updatedPeople);
                return Ok("Person Updated!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }
    }
}
