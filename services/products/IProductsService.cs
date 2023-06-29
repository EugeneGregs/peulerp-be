using PeyulErp.Contracts;

namespace PeyulErp.Services{
    public interface IProductsService{
        Task<IList<GetProductDTO>> GetProductsAsync();
        Task<GetProductDTO> GetProductAsync(Guid id);
        Task<GetProductDTO> CreateProductAsync(SaveProductDTO product);
        Task<bool> UpdateProductAsync(SaveProductDTO productDTO, Guid productId);
        Task<bool> DeleteProductAsync(Guid productId);
    }
}