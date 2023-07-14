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
        private readonly IStockService _stockService;
        private readonly FilterDefinitionBuilder<Product> _filterBuilder = Builders<Product>.Filter;

        public MongoDbProductsService(
            IOptions<MongoDbSettings> mongodbSettings,
            IMongoClient mongoClient,
            IProductCategoryService productCategoryService,
            IStockService stockService)
        {
            var _databaseSettings = mongodbSettings.Value;
            var mongoDatabase = mongoClient.GetDatabase(_databaseSettings.DatabaseName);

            _productsCollection = mongoDatabase.GetCollection<Product>(_databaseSettings.ProductsCollectionName);
            _productCategoryService = productCategoryService;
            _stockService = stockService;
        }

        public async Task<IList<GetProductDTO>> GetProductsAsync()
        {
            var internalProducts = (await _productsCollection.FindAsync(new BsonDocument())).ToList();
            List<GetProductDTO> productDTOs = new();

            foreach(var internalProduct in internalProducts){
                var category = await _productCategoryService.GetProductCategoryAsync(internalProduct.CategoryId);
                productDTOs.Add(internalProduct.AsGetDTO(category));
            }

            return productDTOs;
        }

        public async Task<GetProductDTO> GetProductAsync(Guid Id)
        {
            var filter = _filterBuilder.Eq(p => p.Id, Id);
            var internalProduct = (await _productsCollection.FindAsync(filter)).SingleOrDefault();
            var category = await _productCategoryService.GetProductCategoryAsync(internalProduct.CategoryId);

            return internalProduct?.AsGetDTO(category);
        }

        #pragma warning disable 4014
        public async Task<GetProductDTO> CreateProductAsync(SaveProductDTO product)
        {
            var productInternal = product.AsInternaProduct();
            var category = await _productCategoryService.GetProductCategoryAsync(product.CategoryId);

            await _productsCollection.InsertOneAsync(productInternal);

            //update stock assynchnously
            await UpdateStockAsync(productInternal.Id, product.Quantity, product.ReorderLevel).ConfigureAwait(false);

            return productInternal.AsGetDTO(category);
        }
        #pragma warning restore 4014



        private async Task UpdateStockAsync(Guid productId, int quantity, int reorderLevel)
        {
            var stock = new Stock
            {
                ProductId = productId,
                Quantity = quantity,
                ReorderLevel = reorderLevel
            };

           await _stockService.UpsertStockAsync(stock);
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