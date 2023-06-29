using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using PeyulErp.Models;
using PeyulErp.Settings;

namespace PeyulErp.Services
{
#pragma warning disable 4014
    public class MongoPurchasesService : IPurchasesService
    {
        private IInventoryManager _inventoryManager;
        private IMongoCollection<Purchase> _purchasesCollection;
        private FilterDefinitionBuilder<Purchase> _filter = Builders<Purchase>.Filter;

        public MongoPurchasesService(IInventoryManager inventoryManager, IMongoClient mongoClient, IOptions<MongoDbSettings> mongoSettings)
        {
            var dbSettings = mongoSettings.Value;
            var database = mongoClient.GetDatabase(dbSettings.DatabaseName);

            _purchasesCollection = database.GetCollection<Purchase>(dbSettings.PuchasesCollectionName);
            _inventoryManager = inventoryManager;
        }
        public async Task<bool> DeletePurchase(Guid Id)
        {
            var filter = GetFilterDefinition(Id);
            var purchase = await FindById(Id);

            if (purchase is null)
                return false;

            await _purchasesCollection.DeleteOneAsync(filter);
            return true;
        }

        public async Task<Purchase> GetPurchase(Guid Id)
        {
            return await FindById(Id);
        }

        public async Task<List<Purchase>> GetPurchases()
        {
            return (await _purchasesCollection.FindAsync(new BsonDocument())).ToList();
        }

        public async Task<Purchase> UpsertPurchase(Purchase purchase)
        {
            var existingPurchase = await FindById(purchase.Id);

            if (existingPurchase is null)
            {
                var newPurchase = purchase with { CreateDate = DateTime.UtcNow, Id = Guid.NewGuid() };
                await _purchasesCollection.InsertOneAsync(newPurchase);
                UpdateStockAsync(newPurchase.StockList).ConfigureAwait(false);

                return newPurchase;
            }

            var updated = existingPurchase with {
                Amount = purchase.Amount,
                PaymentType = purchase.PaymentType,
                StockList = purchase.StockList
            };

            await _purchasesCollection.ReplaceOneAsync(GetFilterDefinition(existingPurchase.Id), updated);
            return updated;
        }

        private async Task UpdateStockAsync(IList<Stock> stockList)
        {
            var stockUpdateTasks = new List<Task>();

            foreach (var stock in stockList)
                stockUpdateTasks.Add(_inventoryManager.CheckInProductAsync(stock));

            await Task.WhenAll(stockUpdateTasks);
        }

        private FilterDefinition<Purchase> GetFilterDefinition(Guid Id) => _filter.Eq(p => p.Id, Id);

        private async Task<Purchase> FindById(Guid Id)
        {
            var filter = GetFilterDefinition(Id);
            return (await _purchasesCollection.FindAsync(filter)).SingleOrDefault();
        }
    }
}