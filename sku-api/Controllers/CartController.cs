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

    [HttpGet("")]
    [ProducesResponseType<CartDTO>(StatusCodes.Status200OK)]
    public async Task<ActionResult<CartDTO>> GetCart()
    {
        var cart = await _cartService.GetCartDTO();

        if (cart == null)
        {
            return NotFound();
        }

        return Ok(cart);
    }

    [HttpGet("order")]
    [ProducesResponseType<IEnumerable<OrderDTO>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders()
    {
        var orders = await _cartService.GetOrders();

        if (orders == null)
        {
            return NotFound();
        }

        return Ok(orders);
    }

    [HttpPost("checkout")]
    [ProducesResponseType<UpdateResult>(StatusCodes.Status200OK)]
    public async Task<ActionResult<UpdateResult>> Checkout()
    {
        var order = await _cartService.Checkout();

        if (!order.Success)
        {
            return BadRequest(order);
        }

        return Ok(order);
    }

    [HttpPut("product")]
    [ProducesResponseType<UpdateResult>(StatusCodes.Status200OK)]
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