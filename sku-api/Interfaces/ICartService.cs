public interface ICartService
{
    /// <summary>
    /// Get cart dto
    /// </summary>
    /// <returns>The cart for transfer</returns>
    Task<CartDTO?> GetCartDTO();

    /// <summary>
    /// Get orders
    /// </summary>
    /// <returns>The list of orders for that user</returns>
    Task<IEnumerable<OrderDTO>> GetOrders();

    /// <summary>
    /// Add products to the cart
    /// </summary>
    /// <param name="products"></param>
    /// <returns>Updated cart</returns>
    Task<UpdateResult> PutProducts(IEnumerable<StoreProductDTO> products);

    /// <summary>
    /// Checkout - finalise cart
    /// </summary>
    /// <returns>The order</returns>
    Task<UpdateResult> Checkout();
}