public interface IStoreService
{
    /// <summary> Get Store </summary>
    /// <returns>
    /// Store Product DTOs (products combined with their offers)
    /// </returns>
    Task<IEnumerable<StoreProductDTO>> GetStore();

    /// <summary> Get Products </summary>
    /// <returns>
    /// List of Products
    /// </returns>
    Task<IEnumerable<Product>> GetProducts();

    /// <summary> Get Product </summary>
    /// <param name="id"></param>
    /// <returns>
    /// A product if found
    /// </returns>
    Task<Product?> GetProduct(int id);

    /// <summary> Get Offers </summary>
    /// <returns>
    /// List of offers
    /// </returns>
    Task<IEnumerable<Offer>> GetOffers();

    /// <summary> Get Offer </summary>
    /// <param name="id"></param>
    /// <returns>
    /// Offer if found
    /// </returns>
    Task<Offer?> GetOffer(int id);

    /// <summary> Get Offers </summary>
    /// <param name="productId"></param>
    /// <returns>
    /// Offers if found
    /// </returns>
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