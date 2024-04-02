using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService service)
    {
        _cartService = service;
    }

    /// <summary>
    /// Retrieves the cart details.
    /// </summary>
    /// <returns>The cart details.</returns>
    [HttpGet("")]
    [ProducesResponseType(typeof(CartDTO), StatusCodes.Status200OK)]
    public async Task<ActionResult<CartDTO>> GetCart()
    {
        var cart = await _cartService.GetCartDTO();

        if (cart == null)
        {
            return NotFound();
        }

        return Ok(cart);
    }

    /// <summary>
    /// Gets a list of orders.
    /// </summary>
    /// <returns>A list of orders.</returns>
    [HttpGet("order")]
    [ProducesResponseType(typeof(IEnumerable<OrderDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders()
    {
        var orders = await _cartService.GetOrders();

        if (orders == null)
        {
            return NotFound();
        }

        return Ok(orders);
    }

    /// <summary>
    /// Performs the checkout process.
    /// </summary>
    /// <returns>The result of the checkout.</returns>
    [HttpPost("checkout")]
    [ProducesResponseType(typeof(UpdateResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<UpdateResult>> Checkout()
    {
        var order = await _cartService.Checkout();

        if (!order.Success)
        {
            return BadRequest(order);
        }

        return Ok(order);
    }

    /// <summary>
    /// Updates the products in the cart.
    /// </summary>
    /// <param name="products">The list of products to be updated.</param>
    /// <returns>The updated cart details.</returns>
    [HttpPut("product")]
    [ProducesResponseType(typeof(UpdateResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<UpdateResult>> PutProducts([FromBody] IEnumerable<StoreProductDTO> products)
    {
        var cart = await _cartService.PutProducts(products);

        if (!cart.Success)
        {
            return BadRequest(cart);
        }

        return Ok(cart);
    }
}