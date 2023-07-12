using PeyulErp.Models;

namespace PeyulErp.Services
{
    public interface IDashboardService
    {
        Task<ExpenseSummary> GetExpenseSummary(DateTime startDate, DateTime endDate);
        Task<SalesSummary> GetSalesSummary(DateTime startDate, DateTime endDate);
        Task<PurchaseSummary> GetPurchaseSummary(DateTime startDate, DateTime endDate);
        Task<Dashboard> GetDashboardSummary(DateTime startDate, DateTime endDate);
    }
}