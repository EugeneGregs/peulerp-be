using PeyulErp.Contracts;
using PeyulErp.Models;

namespace PeyulErp.Services {
    public interface IProductCategoryService {
        Task<IList<GetProductCategoryDTO>> GetProductCategoriesAsync();
        Task<ProductCategory> GetProductCategoryAsync(Guid Id);
        Task<GetProductCategoryDTO> CreateProductCategoryAsync(ProductCategoryDTO productCategory);
        Task<bool> UpdateProductCategoryAsync(ProductCategoryDTO productCategory, Guid Id);
        Task<bool> DeleteProductCategoryAsync(Guid Id);
    }
}