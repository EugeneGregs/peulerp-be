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

        public async Task<IList<Transaction>> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                return null;

            if(startDate == endDate)
                endDate = endDate.AddDays(1);

            Console.WriteLine($"Start Date: {startDate} End Date: {endDate}");

            var filter = _filterBuilder.Gte(t => t.CreateDate, startDate.Date) & _filterBuilder.Lte(t => t.CreateDate, endDate.Date);

            return (await _transactionsCollection.FindAsync(filter)).ToList();
        }

        public async Task<SalesSummary> GetSalesSummary(DateTime startDateTime, DateTime endDateTime)
        {
            var startDate = startDateTime.Date;
            var endDate = endDateTime.Date;

            if (startDate > endDate)
                return null;

            if (startDate == endDate)
                endDate = endDate.AddDays(1);

            var SalesSummary = (await _transactionsCollection.Aggregate()
                .Match(t => t.CreateDate >= startDate.Date && t.CreateDate <= endDate.Date)
                .Group(t => 1, g => new SalesSummary
                {
                    TotalSales = g.Sum(t => t.TotalCost),
                    GrossProfit = g.Sum(t => t.TotalMargin),
                    TransactionCount = g.Count()
                }).ToListAsync()).FirstOrDefault() ?? new SalesSummary();

            SalesSummary.MonthlyAggregation = new Dictionary<int, double>();
            SalesSummary.MonthlyAggregation = await GetMonthlySalesAggregation();

            return SalesSummary;
        }

        public async Task<IDictionary<int, double>> GetMonthlySalesAggregation()
        {
            var filter = _filterBuilder.Gte(p => p.CreateDate, new DateTime(DateTime.Now.Year, 1, 1));
            var sales = await _transactionsCollection.FindAsync(filter);

            return sales.ToList().GroupBy(s => s.CreateDate.Month).ToDictionary(g => g.Key, g => (double)g.Sum(s => s.TotalCost));
        }

        public async Task<IDictionary<int, double>> GetMonthlyProfitsAggregation()
        {
            var filter = _filterBuilder.Gte(p => p.CreateDate, new DateTime(DateTime.Now.Year, 1, 1));
            var sales = await _transactionsCollection.FindAsync(filter);

            return sales.ToList().GroupBy(s => s.CreateDate.Month).ToDictionary(g => g.Key, g => (double)g.Sum(s => s.TotalMargin));
        }

        public async Task<IList<Transaction>> GetByPaymentType(PaymentType paymentType)
        {
            var dateFilter = _filterBuilder.And(_filterBuilder.Gte(t => t.CreateDate, DateTime.UtcNow.Date), _filterBuilder.Lt(t => t.CreateDate, DateTime.UtcNow.Date.AddDays(1)));
            var paymentTypeFilter = _filterBuilder.Eq(t => (int)t.PaymentType, (int)paymentType);

            var filter = dateFilter & paymentTypeFilter;

            var transcations = await _transactionsCollection.FindAsync(filter).Result.ToListAsync();

            return transcations;
        }
    }
}