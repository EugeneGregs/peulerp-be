using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using PeyulErp.Contracts;
using PeyulErp.Extensions;
using PeyulErp.Models;
using PeyulErp.Settings;

namespace PeyulErp.Services{
    public class MongoDbProductCategoryService : IProductCategoryService
    {
        private readonly MongoDbSettings _mongodbSettings;
        private readonly IMongoCollection<ProductCategory> _productCategoryCollections;
        private readonly FilterDefinitionBuilder<ProductCategory> filterBuilder = Builders<ProductCategory>.Filter;

        public MongoDbProductCategoryService(
            IOptions<MongoDbSettings> mongoDbConfigs,
            IMongoClient mongoClient)
        {
            _mongodbSettings = mongoDbConfigs.Value;
            var database = mongoClient.GetDatabase(_mongodbSettings.DatabaseName);
            _productCategoryCollections = database.GetCollection<ProductCategory>(_mongodbSettings.ProductCategoriesCollectionName);
        }

        public async Task<GetProductCategoryDTO> CreateProductCategoryAsync(ProductCategoryDTO productCategory)
        {
            var internalProductcategory = productCategory.AsInternal();

            await _productCategoryCollections.InsertOneAsync(internalProductcategory);

            return internalProductcategory.AsDTO();
        }

        public async Task<bool> DeleteProductCategoryAsync(Guid Id)
        {
            var filter = filterBuilder.Eq(p => p.Id, Id);
            var tobeDeleted = await _productCategoryCollections.FindAsync(filter);

            if(tobeDeleted is null){
                return false;
            }

            await _productCategoryCollections.DeleteOneAsync(filter);

            return true;
        }

        public async Task<IList<GetProductCategoryDTO>> GetProductCategoriesAsync()
        {
            var internalCategories = (await _productCategoryCollections.FindAsync(new BsonDocument())).ToList();
            List<GetProductCategoryDTO> productCategoryDTO = new();

            foreach(var internalCategory in internalCategories) {
                productCategoryDTO.Add(internalCategory.AsDTO());
            }

            return productCategoryDTO;
        }

        public async Task<ProductCategory> GetProductCategoryAsync(Guid Id)
        {
            var filter = filterBuilder.Eq(p => p.Id, Id);

            return (await _productCategoryCollections.FindAsync(filter)).SingleOrDefault();
        }

        public async Task<bool> UpdateProductCategoryAsync(ProductCategoryDTO productCategory, Guid Id)
        {
            var filter = filterBuilder.Eq(p => p.Id, Id);
            var existingCategory = (await _productCategoryCollections.FindAsync(filter)).SingleOrDefault();

            if (existingCategory is null){
                return false;
            }

            var newCategory = existingCategory with {
                Name = productCategory.Name,
                UpdateDate = DateTime.Now
            };

            await _productCategoryCollections.ReplaceOneAsync(filter, newCategory);

            return true;
        }
    }
}