using PeyulErp.Contracts;
using PeyulErp.Models;
using PeyulErp.Extensions;

namespace PeyulErp.Services{
    public class InMemoryProductService : IProductsService
    {
        private static readonly List<ProductCategory> _productCategories = new (){
            new ProductCategory { Id = Guid.NewGuid(), Name = "Soaps", CreateDate = DateTime.Now, UpdateDate = DateTime.Now},
            new ProductCategory { Id = Guid.NewGuid(), Name = "Beverages", CreateDate = DateTime.Now, UpdateDate = DateTime.Now},
            new ProductCategory { Id = Guid.NewGuid(), Name = "Saniteries", CreateDate = DateTime.Now, UpdateDate = DateTime.Now}
        };
        private static readonly List<Product> _products = new (){
            new Product { Id = Guid.NewGuid(), CategoryId = _productCategories[0].Id, Name = "Omo 400g", BuyingPrice = 160, SellingPrice = 180, CreateDate = DateTime.Now, UpdateDate = DateTime.Now},
            new Product { Id = Guid.NewGuid(), CategoryId = _productCategories[0].Id, Name = "Sunlight 200g", BuyingPrice = 75, SellingPrice = 85, CreateDate = DateTime.Now, UpdateDate = DateTime.Now},
            new Product { Id = Guid.NewGuid(), CategoryId = _productCategories[1].Id, Name = "Soda 2L", BuyingPrice = 170, SellingPrice = 180, CreateDate = DateTime.Now, UpdateDate = DateTime.Now},
            new Product { Id = Guid.NewGuid(), CategoryId = _productCategories[1].Id, Name = "Afia 500ml", BuyingPrice = 65, SellingPrice = 70, CreateDate = DateTime.Now, UpdateDate = DateTime.Now},
            new Product { Id = Guid.NewGuid(), CategoryId = _productCategories[2].Id, Name = "Sunny Girl Purple", BuyingPrice = 50, SellingPrice = 65, CreateDate = DateTime.Now, UpdateDate = DateTime.Now},
        };

        public async Task<IList<GetProductDTO>> GetProductsAsync()
        {
            return await GetProductDTOsAsync(_products);
        }

        public async Task<GetProductDTO> GetProductAsync(Guid id)
        {
            var product = _products.Find(p => p.Id == id);

            if(product is null){
                return null;
            }

            return product.AsGetDTO(await Task.FromResult(_productCategories.Find(c => c.Id == product.CategoryId)));
        }

        public async Task<GetProductDTO> CreateProductAsync(SaveProductDTO product)
        {
            var internalProduct = product.AsInternaProduct();
            var productCategory = await Task.FromResult(_productCategories.Find(c => c.Id == internalProduct.CategoryId));

            _products.Add(internalProduct);

            return internalProduct.AsGetDTO(productCategory);
        }

        public async Task<bool> UpdateProductAsync(SaveProductDTO productDTO, Guid productId)
        {
            var existingProduct = await Task.FromResult(_products.Find(p => p.Id == productId));

            if(existingProduct is null){
                return false;
            }

            var index = _products.IndexOf(existingProduct);

            var newProduct = existingProduct with {
                Name = productDTO.Name,
                CategoryId = productDTO.CategoryId,
                BarCode = productDTO.BarCode,
                BuyingPrice = productDTO.BuyingPrice,
                SellingPrice = productDTO.SellingPrice,
                UpdateDate = DateTime.Now
            };
            
            _products[index] = newProduct;

            return true;
        }

        public async Task<bool> DeleteProductAsync(Guid productId)
        {
            var Index = await Task.FromResult(_products.FindIndex(p => p.Id == productId));

            if(Index >= 0 ){
                _products.RemoveAt(Index);
                return true;
            } else{
                return false;
            }
        }

        private async Task<IList<GetProductDTO>> GetProductDTOsAsync(IList<Product> products){
            var productDTOList = new List<GetProductDTO>();

            foreach(var product in _products){
                var category = await Task.FromResult(_productCategories.Find(c => c.Id == product.CategoryId));
                var getProductDTO = product.AsGetDTO(category);

                productDTOList.Add(getProductDTO);
            }

            return productDTOList;
        }
    }
}