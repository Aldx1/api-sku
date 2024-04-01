public interface ICartService
{
    Task<CartDTO?> GetCartDTO();
    Task<IEnumerable<OrderDTO>> GetOrders();
    Task<UpdateResult> PutProducts(IEnumerable<StoreProductDTO> products);
    Task<UpdateResult> Checkout();
}