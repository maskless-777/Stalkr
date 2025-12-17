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
    }
}
