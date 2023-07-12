using PeyulErp.Contracts;
using PeyulErp.Extensions;
using PeyulErp.Models;

namespace PeyulErp.Services{
    public class InMemoryProductCategoryService
    {
        private readonly List<ProductCategory> _productCategories;

        public InMemoryProductCategoryService()
        {
            _productCategories = new List<ProductCategory>(){
                new ProductCategory() { Id = Guid.NewGuid(), Name = "Soaps"},
                new ProductCategory() { Id = Guid.NewGuid(), Name = "Bevarages"},
                new ProductCategory() { Id = Guid.NewGuid(), Name = "Cakes"},
                new ProductCategory() { Id = Guid.NewGuid(), Name = "Bread"},
                new ProductCategory() { Id = Guid.NewGuid(), Name = "Biscuits"}
            };
        }

        public GetProductCategoryDTO CreateProductCategoryAsync(ProductCategoryDTO productCategory)
        {
            var internalProductcategory = productCategory.AsInternal();
            _productCategories.Add(internalProductcategory);

            return internalProductcategory.AsDTO();
        }

        public bool DeleteProductCategoryAsync(Guid Id)
        {
            var index = _productCategories.FindIndex( c => c.Id == Id);

            if(index < 0){
                return false;
            }

           _productCategories.RemoveAt(index);

            return true;
        }

        public IList<GetProductCategoryDTO> GetProductCategoriesAsync()
        {
            var result = new List<GetProductCategoryDTO>();

            foreach(var category in _productCategories){
                result.Add(category.AsDTO());
            }

            return result;
        }

        public ProductCategory GetProductCategoryAsync(Guid Id)
        {
            var category = _productCategories.Find(c => c.Id == Id);

            return category;
        }

        public bool UpdateProductCategoryAsync(ProductCategoryDTO productCategory, Guid Id)
        {
            var index = _productCategories.FindIndex(c => c.Id == Id);

            if( index < 0 ){
                return false;
            }

            var updatedProduct = _productCategories[index] with {
                Name = productCategory.Name
            };

            return true;
        }
    }
}