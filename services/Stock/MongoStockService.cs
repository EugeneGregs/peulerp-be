using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using PeyulErp.Models;
using PeyulErp.Settings;

namespace PeyulErp.Services
{
    public class MongoStockService : IStockService
    {
        private readonly IMongoCollection<Stock> _stocksCollection;
        private readonly FilterDefinitionBuilder<Stock> _filterBuilder = Builders<Stock>.Filter;

        public MongoStockService(IMongoClient mongoClient, IOptions<MongoDbSettings> databaseSettings) {
            var settings = databaseSettings.Value;
            var database = mongoClient.GetDatabase(settings.DatabaseName);

            _stocksCollection = database.GetCollection<Stock>(settings.StocksCollectionName);
        }

        //<summary>
        // Find producs whose quantity is below reodering threshold
        // This is to be triggered once on business opening
        //<summary/>
        public async Task<IList<Stock>> GetDiminishingAsync()
        {
            var allStock = await GetStocksAsync();

            return allStock.Where(s => s.Quantity < s.ReorderLevel).ToList();
        }

        //<summary>
        // Checks reoder level by product
        // To be triggered assyncronously during every transaction
        //<summary/>
        public async Task<bool> IsBelowReoderLevel(Guid productId)
        {
            var product = await GetStockByProductIdAsync(productId);
            return product.Quantity < product.ReorderLevel;
        }

        public async Task<Stock> GetStockAsync(Guid stockId)
        {
            var filter = _filterBuilder.Eq(s => s.Id, stockId);
            return (await _stocksCollection.FindAsync(filter)).SingleOrDefault();
        }

        public async Task<Stock> GetStockByProductIdAsync(Guid productId)
        {
            var filter = _filterBuilder.Eq(s => s.ProductId, productId);
            return (await _stocksCollection.FindAsync(filter)).SingleOrDefault();
        }

        public async Task<IList<Stock>> GetStocksAsync()
        {
           return (await _stocksCollection.FindAsync(new BsonDocument())).ToList();
        }

        //TODO: Add Logic to validate product existance
        public async Task<Stock> UpsertStockAsync(Stock stock)
        {
            var filter = _filterBuilder.Eq(s => s.Id, stock.Id);
            var existingStock = (await _stocksCollection.FindAsync(filter)).SingleOrDefault();
            
            if(existingStock != null)
            {
                var newStock = existingStock with
                {
                    Quantity = stock.Quantity,
                    ReorderLevel = stock.ReorderLevel,
                    UpdateDate = DateTime.UtcNow
                };

                await _stocksCollection.ReplaceOneAsync(filter, newStock);
            }
            else
            {
                await _stocksCollection.InsertOneAsync(stock with { CreateDate = DateTime.UtcNow, UpdateDate = DateTime.UtcNow });
            }          

            return stock;
        }

        public Task<bool> DeleteStockAsync(Guid Id)
        {
            throw new NotImplementedException();
        }
    }
}