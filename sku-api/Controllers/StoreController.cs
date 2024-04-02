using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class StoreController : ControllerBase
{
    private readonly IStoreService _storeService;

    public StoreController(IStoreService storeService)
    {
        _storeService = storeService;
    }

    /// <summary>
    /// Retrieves the store 
    /// </summary>
    /// <returns>The product and offers.</returns>
    [HttpGet("")]
    [ProducesResponseType(typeof(IEnumerable<StoreProductDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<StoreProductDTO>>> GetStore()
    {
        var storeProducts = await _storeService.GetStore();

        if (!storeProducts.EmptyIfNull().Any())
        {
            return NotFound("No store products");
        }

        return Ok(storeProducts);
    }

    /// <summary>
    /// Retrieves the products 
    /// </summary>
    /// <returns>The products.</returns>
    [HttpGet("product")]
    [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        var products = await _storeService.GetProducts();

        if (!products.EmptyIfNull().Any())
        {
            return NotFound("No products");
        }

        return Ok(products);
    }

    /// <summary>
    /// Searches for the product
    /// </summary>
    /// <param name="id">The id of the product</param>
    /// <returns>The product if found.</returns>
    [HttpGet("product/{id}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _storeService.GetProduct(id);

        if (product == null)
        {
            return NotFound($"No product with id {id}");
        }

        return Ok(product);
    }

    /// <summary>
    /// Searches for the product offers
    /// </summary>
    /// <param name="productId">The id of the product</param>
    /// <returns>The offers if found.</returns>
    [HttpGet("product/offer/{productId}")]
    [ProducesResponseType(typeof(IEnumerable<Offer>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Offer>>> GetProductOffers(int productId)
    {
        var offers = await _storeService.GetOffers(productId);

        if (!offers.EmptyIfNull().Any())
        {
            return NotFound($"No offers with product id {productId}");
        }

        return Ok(offers);
    }

    /// <summary>
    /// Add products to the inventory
    /// </summary>
    /// <param name="products">The list of products to add</param>
    /// <returns>The updated list of products.</returns>
    [HttpPost("product")]
    [ProducesResponseType(typeof(UpdateResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<UpdateResult>> PostProducts([FromBody] IEnumerable<Product> products)
    {
        if (!products.EmptyIfNull().Any())
        {
            return BadRequest("No products");
        }

        var result = await _storeService.PostProducts(products);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Delete products from the inventory 
    /// </summary>
    /// <param name="productIds">The list of products to remove</param>
    /// <returns>The updated list of products.</returns>
    [HttpDelete("product")]
    [ProducesResponseType(typeof(UpdateResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<UpdateResult>> DeleteProducts([FromBody] IEnumerable<int> productIds)
    {
        var result = await _storeService.DeleteProducts(productIds);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }


    /// <summary>
    /// Retrieve the offers
    /// </summary>
    /// <returns>The list of offers.</returns>
    [HttpGet("offer")]
    [ProducesResponseType(typeof(IEnumerable<Offer>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Offer>>> GetOffers()
    {
        var offers = await _storeService.GetOffers();

        if (!offers.EmptyIfNull().Any())
        {
            return NotFound("No offers found");
        }

        return Ok(offers);
    }

    /// <summary>
    /// Finds the offer
    /// </summary>
    /// <param name="id">The id of the offer</param>
    /// <returns>The offer if found.</returns>
    [HttpGet("offer/{id}")]
    [ProducesResponseType(typeof(Offer), StatusCodes.Status200OK)]
    public async Task<ActionResult<Offer>> GetOffer(int id)
    {
        var offer = await _storeService.GetOffer(id);

        if (offer == null)
        {
            return NotFound($"No offer found with id {id}");
        }

        return Ok(offer);
    }

    /// <summary>
    /// Finds the offer
    /// </summary>
    /// <param name="offers">The list of offers to add</param>
    /// <returns>The updated list of offers.</returns>
    [HttpPost("offer")]
    [ProducesResponseType(typeof(UpdateResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<UpdateResult>> PostOffers([FromBody] IEnumerable<Offer> offers)
    {
        if (!offers.EmptyIfNull().Any())
        {
            return BadRequest("No offers");
        }

        var result = await _storeService.PostOffers(offers);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Deletes the offers
    /// </summary>
    /// <param name="offerIds">The ids of the offers to delete</param>
    /// <returns>The updated list of offers.</returns>
    [HttpDelete("offer")]
    [ProducesResponseType(typeof(UpdateResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<UpdateResult>> DeleteOffers([FromBody] IEnumerable<int> offerIds)
    {
        var result = await _storeService.DeleteOffers(offerIds);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
