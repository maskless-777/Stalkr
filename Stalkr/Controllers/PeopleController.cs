using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stalkr.Models;
using Stalkr.Repositories;

namespace Stalkr.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController(
        IRepository<PeopleModel> repo) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllPeopleAsync()
        {
            try
            {
                Console.WriteLine("In ppl controller");
                var people = await repo.GetAllAsync();
                return Ok(people);
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var product = await repo.FindByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }
                return Ok(product);
            }
            catch (Exception e)
            {
                //logger.LogError(e, $"Error retrieving address with id {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreatePersonAsync(PeopleModel pplModel)
        {
            try
            {
                var numberPeopleCreated = await repo.InsertAsync(pplModel);


                return Ok("Person created");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddressAsync(int id)
        {
            try
            {
                if (await repo.FindByIdAsync(id) == null)
                {
                    return NotFound($"Person with id {id} cannot be found");
                }

                var numberPeopleDeleted = await repo.DeleteAsync(id);

                return Ok("Person Deleted!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //logger.LogError(e.Message, "Error deleting addresss with id {AddressID}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddressAsync(int id, PeopleModel updatedPeople)
        {
            try
            {
                if (await repo.FindByIdAsync(id) == null)
                {
                    return NotFound($"Person with id {id} cannot be found");
                }

                var numberPeopleUpdated = await repo.UpdateAsync(id, updatedPeople);

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
