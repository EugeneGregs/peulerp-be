using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PeyulErp.Contracts;
using PeyulErp.Models;
using PeyulErp.Settings;
using PeyulErp.Extensions;
using MongoDB.Bson;

namespace PeyulErp.Services{
    public class MongoDbProductsService : IProductsService
    {
        private readonly IMongoCollection<Product> _productsCollection;
        private readonly IProductCategoryService _productCategoryService;
        private readonly FilterDefinitionBuilder<Product> _filterBuilder = Builders<Product>.Filter;

        public MongoDbProductsService(
            IOptions<MongoDbSettings> mongodbSettings,
            IMongoClient mongoClient,
            IProductCategoryService productCategoryService)
        {
            var _databaseSettings = mongodbSettings.Value;
            var mongoDatabase = mongoClient.GetDatabase(_databaseSettings.DatabaseName);

            _productsCollection = mongoDatabase.GetCollection<Product>(_databaseSettings.ProductsCollectionName);
            _productCategoryService = productCategoryService;
        }

        public async Task<IList<GetProductDTO>> GetProductsAsync()
        {
            var internalProducts = (await _productsCollection.FindAsync(new BsonDocument())).ToList();
            List<GetProductDTO> productDTOs = new();

            foreach(var internalProduct in internalProducts){
                var category = _productCategoryService.GetProductCategory(internalProduct.CategoryId);
                productDTOs.Add(internalProduct.AsGetDTO(category));
            }

            return productDTOs;
        }

        public async Task<GetProductDTO> GetProductAsync(Guid Id)
        {
            var filter = _filterBuilder.Eq(p => p.Id, Id);
            var internalProduct = (await _productsCollection.FindAsync(filter)).SingleOrDefault();
            var category = _productCategoryService.GetProductCategory(internalProduct.CategoryId);

            return internalProduct?.AsGetDTO(category);
        }

        public async Task<GetProductDTO> CreateProductAsync(SaveProductDTO product)
        {
            var productInternal = product.AsInternaProduct();
            var category = _productCategoryService.GetProductCategory(product.CategoryId);

            await _productsCollection.InsertOneAsync(productInternal);

            return productInternal.AsGetDTO(category);
        }

        public async Task<bool> UpdateProductAsync(SaveProductDTO productDTO, Guid productId)
        {
            var filter = _filterBuilder.Eq(p => p.Id, productId);
            var toUpdate = (await _productsCollection.FindAsync(filter)).SingleOrDefault();

            if(toUpdate is null){
                return false;
            }

            var updated = toUpdate with {
                Name = productDTO.Name,
                BarCode = productDTO.BarCode,
                BuyingPrice = productDTO.BuyingPrice,
                SellingPrice = productDTO.SellingPrice,
                CategoryId = productDTO.CategoryId,
                UpdateDate = DateTime.Now
            };

           await _productsCollection.ReplaceOneAsync(filter, updated);

            return true;
        }

        public async Task<bool> DeleteProductAsync(Guid productId)
        {
            var filter = _filterBuilder.Eq(p => p.Id, productId);
            var tobeDeleted = (await _productsCollection.FindAsync(filter)).SingleOrDefault();

            if(tobeDeleted is null){
                return false;
            }

            await _productsCollection.DeleteOneAsync(filter);

            return true;
        }
    }
}