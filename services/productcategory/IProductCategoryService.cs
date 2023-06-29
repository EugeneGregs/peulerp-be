using PeyulErp.Contracts;
using PeyulErp.Models;

namespace PeyulErp.Services {
    public interface IProductCategoryService {
        IList<GetProductCategoryDTO> GetProductCategories();
        ProductCategory GetProductCategory(Guid Id);
        GetProductCategoryDTO CreateProductCategory(ProductCategoryDTO productCategory);
        bool UpdateProductCategory(ProductCategoryDTO productCategory, Guid Id);
        bool DeleteProductCategory(Guid Id);
    }
}