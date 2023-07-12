using PeyulErp.Models;

namespace PeyulErp.Services
{
    public interface IPurchasesService
    {
        Task<Purchase> GetPurchase(Guid Id);
        Task<List<Purchase>> GetPurchases();
        Task<Purchase> UpsertPurchase(Purchase purchase);
        Task<bool> DeletePurchase(Guid Id);
        Task<List<Purchase>> GetByDateRange(DateTime startDate, DateTime endDate);
        Task<PurchaseSummary> GetPurchaseSummary(DateTime startDate, DateTime endDate);
        Task<IDictionary<int,double>> GetMonthlyTotalPurchases();
    }
}