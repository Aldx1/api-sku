public interface IStoreService
{
    Task<IEnumerable<StoreProductDTO>> GetStore();
    Task<IEnumerable<Product>> GetProducts();
    Task<Product?> GetProduct(int id);
    Task<IEnumerable<Offer>> GetOffers();
    Task<Offer?> GetOffer(int id);
    Task<IEnumerable<Offer>?> GetOffers(int productId);

    /// <summary> Add Products </summary>
    /// <param name="products"></param>
    /// <returns>
    /// Update Result - 
    ///     UpdateResultObject - IEnumerable<Product> (Updated list of products)
    /// </returns>
    Task<UpdateResult> PostProducts(IEnumerable<Product> products);

    /// <summary> Add Offers </summary>
    /// <param name="offers"></param>
    /// <returns>
    /// Update Result - 
    ///     UpdateResultObject - IEnumerable<Offer> (Updated list of offers)
    /// </returns>
    Task<UpdateResult> PostOffers(IEnumerable<Offer> offers);

    /// <summary> Delete Products </summary>
    /// <param name="productIds"></param>
    /// <returns>
    /// Update Result - 
    ///     UpdateResultObject - IEnumerable<Product> (Updated list of products)
    /// </returns>
    Task<UpdateResult> DeleteProducts(IEnumerable<int> productIds);

    /// <summary> Delete Offers </summary>
    /// <param name="offerIds"></param>
    /// <returns>
    /// Update Result - 
    ///     UpdateResultObject - IEnumerable<Offer> (Updated list of offers)
    /// </returns>
    Task<UpdateResult> DeleteOffers(IEnumerable<int> offerIds);
}