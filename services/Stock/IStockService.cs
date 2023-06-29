using PeyulErp.Models;

namespace PeyulErp.Services
{
    public interface IStockService
    {
        Task<Stock> GetStockAsync(Guid stockId);
        Task<Stock> GetStockByProductIdAsync(Guid productId);
        Task<IList<Stock>> GetStocksAsync();
        Task<bool> DeleteStockAsync(Guid Id);
        Task UpsertStockAsync(Stock stock);
        Task<IList<Stock>> GetDiminishingAsync();
        Task<bool> IsBelowReoderLevel(Guid productId);
    }
}