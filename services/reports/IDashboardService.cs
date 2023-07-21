using PeyulErp.Models;

namespace PeyulErp.Services
{
    public interface IDashboardService
    {
        Task<ExpenseSummary> GetExpenseSummaryAsync(DateTime startDate, DateTime endDate);
        Task<SalesSummary> GetSalesSummaryAsync(DateTime startDate, DateTime endDate);
        Task<PurchaseSummary> GetPurchaseSummaryAsync(DateTime startDate, DateTime endDate);
        Task<Dashboard> GetDashboardSummaryAsync(DateTime startDate, DateTime endDate);
    }
}