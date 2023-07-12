//---------------------------------------------------------------------
// <copyright file="DefaultInventoryManager.cs" company="PEUL Africa">
//   Copyright (c) PEUL Africa. All rights reserved.
// </copyright>
//--------------------------------------------------------------------

using Microsoft.Extensions.Options;
using PeyulErp.Contracts;
using PeyulErp.Models;
using PeyulErp.Settings;
using System.Security.Cryptography;

namespace PeyulErp.Services
{
    //<inheritdoc/>
#pragma warning disable 4014
    public class DefaultInventoryManager : IInventoryManager
    {
        private IStockService _stockService;
        private IList<GetProductDTO> _expiringProducts;
        private IList<GetProductDTO> _diminishingProducts;
        private IProductsService _productsService;
        private readonly SystemSettings _systemSettings;
        private bool _isRefreshing = false;

        public DefaultInventoryManager(IStockService stockService, IProductsService productsService, IOptions<SystemSettings> systemSettings)
        {
            _stockService = stockService;
            _productsService = productsService;
            _systemSettings = systemSettings.Value;
            _expiringProducts = new List<GetProductDTO>();
            _diminishingProducts = new List<GetProductDTO>();

            //Loads the diminishing list(Fires an async call and forgets)
            RefreshDiminishingProductsAsync().ConfigureAwait(false);
        }

        //<Summary>
        //To be triggered during a purchase or sales return
        //<summary/>
        public async Task<Stock> CheckInProductAsync(Stock newStock)
        {
            var stock = await _stockService.GetStockByProductIdAsync(newStock.ProductId);

            if (stock is not null)
                stock.Quantity += newStock.Quantity;
            else
                stock = GetNewStock(newStock.ProductId, newStock.Quantity);

            await _stockService.UpsertStockAsync(stock);

            UpdateDiminishingListAsync(stock).ConfigureAwait(false);

            return stock;
        }

        //<Summary>
        //To be triggered during a sale or purchase return
        //<summary/>
        public async Task CheckOutProductAsync(Guid productId, int quantity)
        {
            var stock = await _stockService.GetStockByProductIdAsync(productId);

            if (stock is not null && stock.Quantity >= 0)
            {
                var newQuantity = stock.Quantity - quantity;
                stock.Quantity = newQuantity > 0 ? newQuantity : 0;
                UpdateDiminishingListAsync(stock).ConfigureAwait(false);
                await _stockService.UpsertStockAsync(stock);
            }     
        }

        public async Task<IList<GetProductDTO>> GetDiminishedProductsAsync(bool forceRefresh = false)
        {
            if (forceRefresh || _diminishingProducts.Count == 0)
            {
                _diminishingProducts.Clear();
                await RefreshDiminishingProductsAsync();
            }

            return _diminishingProducts;
        }

        public IList<GetProductDTO> GetExpiringProducts()
        {
            return _expiringProducts;
        }

        public async Task RefreshDiminishingProductsAsync()
        {
            while (_isRefreshing)
            {
                if(!_isRefreshing)
                    break;
            }

            _isRefreshing = true;
            Console.WriteLine("Begin Refreshing Dinishing List..");
            var diminishingStock = await _stockService.GetDiminishingAsync();
            var tasks = new List<Task>();

            foreach (var stock in diminishingStock)
                tasks.Add(AddProductToDiminishingListAsync(stock.ProductId));

            await Task.WhenAll(tasks);
            _isRefreshing = false;
            Console.WriteLine($"Done Refreshing Reshreshing Diminisng List.. Count: {_diminishingProducts}");
        }

        private Stock GetNewStock(Guid productId, int quantity, int reorderLevel = 0) => new Stock
        {
            CreateDate = DateTime.Now,
            Id = Guid.NewGuid(),
            ProductId = productId,
            Quantity = quantity,
            ReorderLevel = reorderLevel == 0 ? _systemSettings.DefaultReorderLevel : reorderLevel
        };

        public async Task AddProductToDiminishingListAsync(Guid productId)
        {
            var product = await _productsService.GetProductAsync(productId);

            if (product is not null && !IsProductInDiminishingList(productId))
                _diminishingProducts.Add(product);          
        }

        private bool IsProductInDiminishingList(Guid productId) => _diminishingProducts.SingleOrDefault(p => p.Id == productId) != null;

        public void RemoveProductFromDiminishingList(Guid productId)
        {
            var product = _diminishingProducts.FirstOrDefault(p => p.Id == productId);

            if (product is not null)
                _diminishingProducts.Remove(product);
        }

        private async Task UpdateDiminishingListAsync(Stock stock)
        {
            var isDiminished = stock.Quantity < stock.ReorderLevel;
            var isInDiminishedList = _diminishingProducts.SingleOrDefault(p => p.Id == stock.ProductId) != null;

            if (isDiminished && !isInDiminishedList)
                await AddProductToDiminishingListAsync(stock.ProductId);

            if(!isDiminished && isInDiminishedList)
                RemoveProductFromDiminishingList(stock.ProductId);
        }

        public async Task<IList<Inventory>> GetCurrentStockListAsync()
        {
            var inventoryList = new List<Inventory>();
            var stockList = await _stockService.GetStocksAsync();
            var fetchInventoryTask = new List<Task>();

            foreach (var stock in stockList)
                fetchInventoryTask.Add(PopulateInventory(inventoryList, stock));

            await Task.WhenAll(fetchInventoryTask);

            return inventoryList;
        }

        private async Task PopulateInventory(IList<Inventory> inventoryList, Stock stock)
        {
            var product  = await _productsService.GetProductAsync(stock.ProductId);
            inventoryList.Add(new Inventory
            {
                Id = product.Id,
                Name = product.Name,
                ProductCategory = product.ProductCategory,
                BuyingPrice = product.BuyingPrice,
                SellingPrice = product.SellingPrice,
                Quantity = stock.Quantity,
                CreatedDate = product.CreatedDate,
                UpdatedDate = product.UpdatedDate,
                BarCode = product.BarCode
            });
        }
    }
}