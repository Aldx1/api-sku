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

    public StoreController(IStoreService service)
    {
        _storeService = service;
    }

    [HttpGet("")]
    [ProducesResponseType<IEnumerable<StoreProductDTO>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<StoreProductDTO>>> GetStore()
    {
        var storeProducts = await _storeService.GetStore();

        if (storeProducts == null)
        {
            return NotFound();
        }

        return Ok(storeProducts);
    }

    [HttpGet("product")]
    [ProducesResponseType<IEnumerable<Product>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        var products = await _storeService.GetProducts();

        if (products == null || products.Count() == 0)
        {
            return NotFound();
        }

        return Ok(products);
    }

    [HttpGet("product/{id}")]
    [ProducesResponseType<Product>(StatusCodes.Status200OK)]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _storeService.GetProduct(id);

        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }


    [HttpGet("product/offer/{productId}")]
    [ProducesResponseType<IEnumerable<Offer>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Offer>>> GetProductOffers(int productId)
    {
        var offers = await _storeService.GetOffers(productId);

        if (offers == null)
        {
            return NotFound();
        }

        return Ok(offers);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="products"></param>
    /// <returns>
    /// Update Result 
    ///     UpdateResultObject = Updated List Of Products
    /// </returns>
    [HttpPost("product")]
    [ProducesResponseType<UpdateResult>(StatusCodes.Status200OK)]
    public async Task<ActionResult<UpdateResult>> PostProducts([FromBody] IEnumerable<Product> products)
    {
        var result = await _storeService.PostProducts(products);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productIds"></param>
    /// <returns>
    /// Update Result 
    ///     UpdateResultObject = Updated List Of Products and offers
    /// </returns>
    [HttpDelete("product")]
    [ProducesResponseType<UpdateResult>(StatusCodes.Status200OK)]
    public async Task<ActionResult<UpdateResult>> DeleteProducts([FromBody] IEnumerable<int> productIds)
    {
        var result = await _storeService.DeleteProducts(productIds);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("offer")]
    [ProducesResponseType<IEnumerable<Offer>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Offer>>> GetOffers()
    {
        var offers = await _storeService.GetOffers();

        if (offers == null)
        {
            return NotFound();
        }

        return Ok(offers);
    }

    [HttpGet("offer/{id}")]
    [ProducesResponseType<Offer>(StatusCodes.Status200OK)]
    public async Task<ActionResult<Offer>> GetOffer(int id)
    {
        var offer = await _storeService.GetOffer(id);

        if (offer == null)
        {
            return NotFound();
        }

        return Ok(offer);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="offers"></param>
    /// <returns>
    /// Update Result 
    ///     UpdateResultObject = Updated List Of Offers
    /// </returns>
    [HttpPost("offer")]
    [ProducesResponseType<UpdateResult>(StatusCodes.Status200OK)]
    public async Task<ActionResult<UpdateResult>> PostOffers([FromBody] IEnumerable<Offer> offers)
    {
        var result = await _storeService.PostOffers(offers);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="offerIds"></param>
    /// <returns>
    /// Update Result 
    ///     UpdateResultObject = Updated List Of Offers
    /// </returns>
    [HttpDelete("offer")]
    [ProducesResponseType<UpdateResult>(StatusCodes.Status200OK)]
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