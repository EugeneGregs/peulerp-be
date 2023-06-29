using PeyulErp.Contracts;
using PeyulErp.Models;
//---------------------------------------------------------------------
// <copyright file="IInventoryManager.cs" company="PEUL Africa">
//   Copyright (c) PEUL Africa. All rights reserved.
// </copyright>
//--------------------------------------------------------------------

namespace PeyulErp.Services
{
    public interface IInventoryManager
    {
        // <summary>
        //  Increments the stock quantity for a given product
        // </summary
        Task<Stock> CheckInProductAsync(Stock stock);

        // <summary>
        //  Decrements the stock quantity for a given product
        // </summary
        Task CheckOutProductAsync(Guid productId, int quantity);

        // <summary>
        //  Gets all the products that a re nearing expiry.
        // </summary
        IList<GetProductDTO> GetExpiringProducts();

        // <summary>
        //  Gets all the products whose stock quantity is below the reorder level.
        // </summary
        Task<IList<GetProductDTO>> GetDiminishedProductsAsync(bool forceRefresh = false);
        Task RefreshDiminishingProductsAsync();
        Task AddProductToDiminishingListAsync(Guid productId);
        void RemoveProductFromDiminishingList(Guid productId);
        Task<IList<Inventory>> GetCurrentStockListAsync();
    }
}