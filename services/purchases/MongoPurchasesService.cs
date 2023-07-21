using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using PeyulErp.Contracts;
using PeyulErp.Models;
using PeyulErp.Settings;
using System.Globalization;
using System.Runtime.Serialization;

namespace PeyulErp.Services
{
#pragma warning disable 4014
    public class MongoPurchasesService : IPurchasesService
    {
        private readonly IInventoryManager _inventoryManager;
        private readonly IMongoCollection<Purchase> _purchasesCollection;
        private readonly FilterDefinitionBuilder<Purchase> _filter = Builders<Purchase>.Filter;
        private readonly IProductsService _productsService;

        public MongoPurchasesService(
            IInventoryManager inventoryManager,
            IMongoClient mongoClient,
            IOptions<MongoDbSettings> mongoSettings,
            IProductsService productsService
            )
        {
            var dbSettings = mongoSettings.Value;
            var database = mongoClient.GetDatabase(dbSettings.DatabaseName);

            _purchasesCollection = database.GetCollection<Purchase>(dbSettings.PuchasesCollectionName);
            _inventoryManager = inventoryManager;
            _productsService = productsService;
        }
        public async Task<bool> DeletePurchase(Guid Id)
        {
            var filter = GetFilterDefinition(Id);
            var purchase = await FindById(Id);

            if (purchase is null)
                return false;

            var updatedStockList = GetUpdatedStockList(purchase.PurchaseProducts, new List<PurchaseProduct>());

            if (updatedStockList.Count > 0)
                UpdateStockAsync(updatedStockList).ConfigureAwait(false);

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

                var stockList = new List<Stock>();

                foreach (var purchaseProduct in newPurchase.PurchaseProducts)
                    stockList.Add( new Stock { ProductId = purchaseProduct.ProductId, Quantity = purchaseProduct.Quantity, ReorderLevel = purchaseProduct.ReorderLevel});

                UpdateStockAsync(stockList).ConfigureAwait(false);
                UpdateProductListPriceAsync(newPurchase.PurchaseProducts).ConfigureAwait(false);

                return newPurchase;
            }

            var updated = existingPurchase with {
                Amount = purchase.Amount,
                PaymentStatus = purchase.PaymentStatus,
                PurchaseProducts = purchase.PurchaseProducts,
                SupplierId = purchase.SupplierId,
                PurchaseDate = purchase.PurchaseDate,
                Description = purchase.Description,
                RefferenceNo = purchase.RefferenceNo,
                PaymentType = purchase.PaymentType,
                UpdateDate = DateTime.UtcNow
            };

            await _purchasesCollection.ReplaceOneAsync(GetFilterDefinition(existingPurchase.Id), updated);
            UpdateProductListPriceAsync(updated.PurchaseProducts).ConfigureAwait(false);

            var updatedStockList = GetUpdatedStockList(existingPurchase.PurchaseProducts, updated.PurchaseProducts);

            if(updatedStockList.Count > 0)
                UpdateStockAsync(updatedStockList).ConfigureAwait(false);

            return updated;
        }

        private List<Stock> GetUpdatedStockList(List<PurchaseProduct> previusProductList, List<PurchaseProduct> updatedProductList)
        {
            List<Stock> stocksToUpdate = new();

            //If the product price list changed, update the stock
            foreach (var updatedProduct in updatedProductList)
            {
                var previousProduct = previusProductList.FirstOrDefault(x => x.ProductId == updatedProduct.ProductId);

                if (previousProduct is not null)
                {
                    if(previousProduct.Quantity != updatedProduct.Quantity)
                        stocksToUpdate.Add(new Stock { ProductId = updatedProduct.ProductId, Quantity = updatedProduct.Quantity - previousProduct.Quantity });                
                } else
                {
                    stocksToUpdate.Add(new Stock { ProductId = updatedProduct.ProductId, Quantity = updatedProduct.Quantity });
                }
            }

            //If the product was deleted, reduce the stock quantity
            foreach (var previousProduct in previusProductList)
            {
                var updatedProduct = updatedProductList.FirstOrDefault(x => x.ProductId == previousProduct.ProductId);

                if (updatedProduct is null)
                {
                    stocksToUpdate.Add(new Stock { ProductId = previousProduct.ProductId, Quantity = -previousProduct.Quantity});
                }
            }

            return stocksToUpdate;
        }

        private async Task UpdateProductListPriceAsync(IList<PurchaseProduct> purchaseProducts)
        {
            var productUpdateTasks = new List<Task>();

            foreach (var purchaseProduct in purchaseProducts)
                productUpdateTasks.Add(UpdateProdutPriceAsync(purchaseProduct));

            await Task.WhenAll(productUpdateTasks);
        }

        private async Task UpdateProdutPriceAsync(PurchaseProduct purchaseProduct)
        {
            var product = await _productsService.GetProductAsync(purchaseProduct.ProductId);

            if (ShouldUpdateProductPrice(product, purchaseProduct))
            {
                var updatedProduct = new SaveProductDTO
                {
                    Name = product.Name,
                    BuyingPrice = purchaseProduct.PurchasePrice,
                    SellingPrice = product.SellingPrice,
                    CategoryId = product.ProductCategory.Id,
                    BarCode = product.BarCode
                };

                await _productsService.UpdateProductAsync(updatedProduct, product.Id);
            }          
        }

        private bool ShouldUpdateProductPrice(GetProductDTO existingProduct, PurchaseProduct newProduct) => existingProduct is not null && existingProduct.BuyingPrice != newProduct.PurchasePrice;

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

        public async Task<List<Purchase>> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                return null;

            if(startDate == endDate)
                endDate = endDate.AddDays(1);

            var filter = _filter.Gte(p => p.PurchaseDate, startDate.Date) & _filter.Lte(p => p.PurchaseDate, endDate.Date);

            return await _purchasesCollection.Find(filter).ToListAsync();
        }

        public async Task<PurchaseSummary> GetPurchaseSummary(DateTime startDateTime, DateTime endDateTime)
        {
            var startDate = startDateTime.Date;
            var endDate = endDateTime.Date;

            if(startDate > endDate)
                return null;

            if(startDate == endDate)
                endDate = endDate.AddDays(1);

            var filter = _filter.Gte(p => p.PurchaseDate, startDate) & _filter.Lte(p => p.PurchaseDate, endDate);

            var purchaseSummary = (await _purchasesCollection.Aggregate()
                .Match(filter)
                .Group(p => 1, g => new PurchaseSummary
                {
                    TotalPurchases = g.Sum(p => p.Amount),
                    PurchaseCount = g.Count()
                }).ToListAsync()).FirstOrDefault() ?? new PurchaseSummary();

            purchaseSummary.MonthlyPurchaseTotals = new Dictionary<int, double>();
            purchaseSummary.MonthlyPurchaseTotals = await GetMonthlyTotalPurchases();

            return purchaseSummary;
        }

        public async Task<IDictionary<int,double>> GetMonthlyTotalPurchases()
        {
            var filter = _filter.Gte(p => p.PurchaseDate, new DateTime(DateTime.Now.Year, 1, 1));

            var thisYearPurchases = (await _purchasesCollection.FindAsync(filter)).ToList();
            
            return thisYearPurchases.GroupBy(p => p.PurchaseDate.Month).ToDictionary(g => g.Key, g => (double)g.Sum(p => p.Amount));
        }

        public async Task<IList<Purchase>> GetByPaymentType(PaymentType paymentType)
        {
            var dateFilter = _filter.And(_filter.Gte(p => p.PurchaseDate, DateTime.UtcNow.Date), _filter.Lt(p => p.PurchaseDate, DateTime.UtcNow.Date.AddDays(1)));
            var paymentTypeFilter = _filter.Eq(p => p.PaymentType, paymentType);

            var filter = dateFilter & paymentTypeFilter;

            return (await _purchasesCollection.FindAsync(filter)).ToList();
        }
    }
}