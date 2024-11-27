using Microsoft.AspNetCore.Mvc;
using SaleCheck.DataAccess;
using SaleCheck.Model.DataClasses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaleCheck.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScraperWebsiteRulesController : ControllerBase
    {
        private readonly ScraperWebsiteRulesRepository _repository;

        public ScraperWebsiteRulesController(ScraperWebsiteRulesRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> Create(ScraperWebsiteRules rules)
        {
            if (rules == null)
                return BadRequest("Invalid data.");

            await _repository.CreateAsync(rules);
            return Ok("ScraperWebsiteRules created successfully.");
        }

        [HttpGet]
        public async Task<ActionResult<List<ScraperWebsiteRules>>> GetAll()
        {
            var result = await _repository.GetAllAsync();
            return Ok(result);
        }
    }
}