using Microsoft.AspNetCore.Mvc;

namespace SaleCheck.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnalyseController : ControllerBase
    {
        [HttpPost("HTML")]
        public ActionResult<int> DetectSaleFromHTML([FromBody] string htmlFile)
        {
            if (string.IsNullOrEmpty(htmlFile))
            {
                // Return a 400 Bad Request if the input is null or empty
                return BadRequest("Invalid HTML content.");
            }

            int salePercentage = DetectSalePercentage(htmlFile);

            return Ok(salePercentage);
        }

        //The basics of basic logic. Replace with proper logic. Just here to open the endpoint to test that.
        private int DetectSalePercentage(string htmlContent)
        {
            if (htmlContent.Contains("50% off", StringComparison.OrdinalIgnoreCase))
            {
                return 50;
            }
            
            else if (htmlContent.Contains("Sale", StringComparison.OrdinalIgnoreCase))
            {
                return 10;
            }
            return 0;
        }
    }
}
