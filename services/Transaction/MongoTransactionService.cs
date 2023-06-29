//---------------------------------------------------------------------
// <copyright file="MongoTransactionService.cs" company="PEUL Africa">
//   Copyright (c) PEUL Africa. All rights reserved.
// </copyright>
//--------------------------------------------------------------------

using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using PeyulErp.Models;
using PeyulErp.Settings;

namespace PeyulErp.Services
{
#pragma warning disable 4014
    public class MongoTransactionService : ITransactionService
    {
        private readonly IMongoCollection<Transaction> _transactionsCollection;
        private readonly FilterDefinitionBuilder<Transaction> _filterBuilder = Builders<Transaction>.Filter;
        private readonly IInventoryManager _inventoryManager;

        public MongoTransactionService(IOptions<MongoDbSettings> mongodbSettings, IMongoClient mongoClient, IInventoryManager inventoryManager)
        {
            var _databaseSettings = mongodbSettings.Value;
            var mongoDatabase = mongoClient.GetDatabase(_databaseSettings.DatabaseName);

            _transactionsCollection = mongoDatabase.GetCollection<Transaction>(_databaseSettings.TransactionsCollectionName);
            _inventoryManager = inventoryManager;
        }

        public async Task<Transaction> CreateTransaction(Transaction transaction)
        {
            transaction.Id = Guid.NewGuid();
            transaction.CreateDate = DateTime.UtcNow;
            await _transactionsCollection.InsertOneAsync(transaction);
            UpdateStock(transaction.CartItems).ConfigureAwait(false);

            return transaction;
        }

        public async Task<bool> DeleteTransaction(Guid transactionId)
        {
            var filter = _filterBuilder.Eq(t => t.Id, transactionId);
            var tobeDeleted = (await _transactionsCollection.FindAsync(filter)).SingleOrDefault();

            if (tobeDeleted is null)
            {
                return false;
            }

            await _transactionsCollection.DeleteOneAsync(filter);

            return true;
        }

        public async Task<Transaction> GetTransaction(Guid transactionId)
        {
            var filter = _filterBuilder.Eq(t => t.Id, transactionId);
            var transaction = (await _transactionsCollection.FindAsync(filter)).SingleOrDefault();

            return transaction;
        }

        private async Task UpdateStock(IList<CartItem> products)
        {
            var updateTasks = new List<Task>();

            foreach (var product in products)
                updateTasks.Add(_inventoryManager.CheckOutProductAsync(product.ProductId, product.Quantity));

            await Task.WhenAll(updateTasks);
        }

        public async Task<IList<Transaction>> GetTransactions()
        {
           return (await _transactionsCollection.FindAsync(new BsonDocument())).ToList();
        }

        public Task<bool> UpdateTransaction(Transaction transaction)
        {
            throw new NotImplementedException();
        }
    }
}