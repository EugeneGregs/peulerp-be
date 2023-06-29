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

        public GetProductCategoryDTO CreateProductCategory(ProductCategoryDTO productCategory)
        {
            var internalProductcategory = productCategory.AsInternal();

            _productCategoryCollections.InsertOne(internalProductcategory);

            return internalProductcategory.AsDTO();
        }

        public bool DeleteProductCategory(Guid Id)
        {
            var filter = filterBuilder.Eq(p => p.Id, Id);
            var tobeDeleted = _productCategoryCollections.Find(filter);

            if(tobeDeleted is null){
                return false;
            }

            _productCategoryCollections.DeleteOne(filter);

            return true;
        }

        public IList<GetProductCategoryDTO> GetProductCategories()
        {
            var internalCategories = _productCategoryCollections.Find(new BsonDocument()).ToList();
            List<GetProductCategoryDTO> productCategoryDTO = new();

            foreach(var internalCategory in internalCategories) {
                productCategoryDTO.Add(internalCategory.AsDTO());
            }

            return productCategoryDTO;
        }

        public ProductCategory GetProductCategory(Guid Id)
        {
            var filter = filterBuilder.Eq(p => p.Id, Id);

            return _productCategoryCollections.Find(filter).SingleOrDefault();
        }

        public bool UpdateProductCategory(ProductCategoryDTO productCategory, Guid Id)
        {
            var filter = filterBuilder.Eq(p => p.Id, Id);
            var existingCategory = _productCategoryCollections.Find(filter).SingleOrDefault();

            if (existingCategory is null){
                return false;
            }

            var newCategory = existingCategory with {
                Name = productCategory.Name,
                UpdateDate = DateTime.Now
            };

            _productCategoryCollections.ReplaceOne(filter, newCategory);

            return true;
        }
    }
}