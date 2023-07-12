using PeyulErp.Models;

namespace PeyulErp.Services
{
    public interface IExpenseService
    {
        Task<IEnumerable<Expense>> GetExpensesAsync();
        Task<Expense> GetExpenseAsync(Guid Id);
        Task<Expense> CreateExpenseAsync(Expense expense);
        Task<Expense> UpdateExpenseAsync(Expense expense);
        Task<bool> DeleteExpenseAsync(Guid Id);
        Task<IList<Expense>> GetByDateRange(DateTime startDate, DateTime endDate);
        Task<ExpenseSummary> GetExpenseSummary(DateTime startDate, DateTime endDate);
        Task<IDictionary<int,double>> GetMonthlyAggregation();
    }   
}  