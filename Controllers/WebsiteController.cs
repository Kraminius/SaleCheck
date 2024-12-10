using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaleCheck.Model.DataClasses;
using SaleCheck.Model.Utility;
using SaleCheck.Repositories.Interfaces;

namespace SaleCheck.Controllers;

[ApiController]
[Route("[controller]")]
public class WebsiteController : ControllerBase
{
    private readonly IWebsiteRepository _websiteRepository;
    private readonly DataFactory _dataFactory;
    private readonly WebsiteSummaryService _websiteSummaryService;
    private readonly ILogger<WebsiteController> _logger;

    public WebsiteController(IWebsiteRepository websiteRepository, DataFactory dataFactory, WebsiteSummaryService websiteSummaryService, ILogger<WebsiteController> logger)
    {
        _websiteRepository = websiteRepository;
        _dataFactory = dataFactory;
        _websiteSummaryService = websiteSummaryService;
        _logger = logger;
    }

    #region Website endpoints

    /// <summary>
    /// Retrieves all available websites.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Website>>> GetWebsites()
    {
        var websites = await _websiteRepository.GetAllWebsitesAsync();
        return Ok(websites);
    }

    /// <summary>i
    /// Retrieves a website by its ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Website>> GetWebsiteById(string id)
    {
        var website = await _websiteRepository.GetWebsiteByIdAsync(id);
        if (website == null)
        {
            return NotFound($"Website with ID {id} not found.");
        }

        return Ok(website);
    }

    /// <summary>
    /// Creates a new website.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateWebsite([FromBody] WebsiteInit website)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
           

        try
        {
            await _dataFactory.CreateWebsite(website);
            return Ok("Website created successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
    

    // <summary>
    /// Updates an existing website.
    /// </summary>
    [RestrictToLocalNetwork]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateWebsite(string id, [FromBody] Website website)
    {
        if (website == null || !website.WebsiteId.Equals(id))
            return BadRequest("Website ID mismatch.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _websiteRepository.UpdateWebsiteAsync(id, website);
            return NoContent();
        }
        catch (KeyNotFoundException knfEx)
        {
            return NotFound(knfEx.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // <summary>
    /// Deletes a website by its ID.
    /// </summary>
    [RestrictToLocalNetwork]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWebsite(string id)
    {
        try
        {
            await _websiteRepository.DeleteWebsiteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException knfEx)
        {
            return NotFound(knfEx.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    #endregion Website endpoints

    #region Product Endpoints

    /// <summary>
    /// Retrieves all products for a specific website.
    /// </summary>
    [HttpGet("{websiteId}/products")]
    public async Task<IActionResult> GetProducts(string websiteId)
    {
        var products = await _websiteRepository.GetProductsByWebsiteIdAsync(websiteId);
        return Ok(products);
    }

    /// <summary>
    /// Retrieves a specific product by ID within a website.
    /// </summary>
    [HttpGet("{websiteId}/products/{productId}")]
    public async Task<IActionResult> GetProductById(string websiteId, string productId)
    {
        var product = await _websiteRepository.GetProductByIdAsync(websiteId, productId);
        if (product == null)
            return NotFound($"Product with ID {productId} not found in website ID {websiteId}.");
        return Ok(product);
    }

    /// <summary>
    /// Updates an existing product within a website.
    /// </summary>
    [RestrictToLocalNetwork]
    [HttpPut("{websiteId}/products/{productId}")]
    public async Task<IActionResult> UpdateProduct(string websiteId, string productId, [FromBody] Product product)
    {
        if (product == null || !product.ProductId.Equals(productId))
            return BadRequest("Product ID mismatch.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _websiteRepository.UpdateProductAsync(websiteId, productId, product);
            return NoContent();
        }
        catch (KeyNotFoundException knfEx)
        {
            return NotFound(knfEx.Message);
        }
        catch (Exception ex)
        {
            // Log exception if not already handled in repository
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Deletes a product from a website.
    /// </summary>
    [RestrictToLocalNetwork]
    [HttpDelete("{websiteId}/products/{productId}")]
    public async Task<IActionResult> DeleteProduct(string websiteId, string productId)
    {
        try
        {
            await _websiteRepository.DeleteProductAsync(websiteId, productId);
            return NoContent();
        }
        catch (KeyNotFoundException knfEx)
        {
            return NotFound(knfEx.Message);
        }
        catch (Exception ex)
        {
            // Log exception if not already handled in repository
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
    [RestrictToLocalNetwork]
    [HttpPost("{websiteId}/products")]
    public async Task<IActionResult> AddProduct(string websiteId, [FromBody] Product product)
    {
        if (string.IsNullOrEmpty(websiteId))
        {
            return BadRequest("WebsiteId cannot be null or empty.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _websiteRepository.CreateProductAsync(websiteId, product);
            return Ok($"Product with ID: {product.ProductId} added successfully to Website ID: {websiteId}.");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    #endregion

    #region Subsite Endpoints

    /// <summary>
    /// Retrieves all subsites for a specific website.
    /// </summary>
    [HttpGet("{websiteId}/subsites")]
    public async Task<IActionResult> GetSubsites(string websiteId)
    {
        var subsites = await _websiteRepository.GetSubsitesByWebsiteIdAsync(websiteId);
        return Ok(subsites);
    }

    /// <summary>
    /// Retrieves a specific subsite by URL within a website.
    /// </summary>
    [HttpGet("{websiteId}/subsites/{url}")]
    public async Task<IActionResult> GetSubsiteByUrl(string websiteId, string url)
    {
        var subsite = await _websiteRepository.GetSubsiteByUrlAsync(websiteId, url);
        if (subsite == null)
            return NotFound($"Subsite with URL {url} not found in website ID {websiteId}.");
        return Ok(subsite);
    }

    /// <summary>
    /// Creates a new subsite within a website.
    /// </summary>
    [RestrictToLocalNetwork]
    [HttpPost("{websiteId}/subsites")]
    public async Task<IActionResult> CreateSubsite(string websiteId, [FromBody] Subsite subsite)
    {
        if (subsite == null)
            return BadRequest("Subsite data is null.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _websiteRepository.CreateSubsiteAsync(websiteId, subsite);
            return CreatedAtAction(nameof(GetSubsiteByUrl), new { websiteId = websiteId, url = subsite.Url }, subsite);
        }
        catch (KeyNotFoundException knfEx)
        {
            return NotFound(knfEx.Message);
        }
        catch (Exception ex)
        {
            // Log exception if not already handled in repository
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates an existing subsite within a website.
    /// </summary>
    [RestrictToLocalNetwork]
    [HttpPut("{websiteId}/subsites/{url}")]
    public async Task<IActionResult> UpdateSubsite(string websiteId, string url, [FromBody] Subsite subsite)
    {
        if (subsite == null || !subsite.Url.Equals(url))
            return BadRequest("Subsite URL mismatch.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _websiteRepository.UpdateSubsiteAsync(websiteId, url, subsite);
            return NoContent();
        }
        catch (KeyNotFoundException knfEx)
        {
            return NotFound(knfEx.Message);
        }
        catch (Exception ex)
        {
            // Log exception if not already handled in repository
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Deletes a subsite from a website.
    /// </summary>
    [RestrictToLocalNetwork]
    [HttpDelete("{websiteId}/subsites/{url}")]
    public async Task<IActionResult> DeleteSubsite(string websiteId, string url)
    {
        try
        {
            await _websiteRepository.DeleteSubsiteAsync(websiteId, url);
            return NoContent();
        }
        catch (KeyNotFoundException knfEx)
        {
            return NotFound(knfEx.Message);
        }
        catch (Exception ex)
        {
            // Log exception if not already handled in repository
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    #endregion
    
}