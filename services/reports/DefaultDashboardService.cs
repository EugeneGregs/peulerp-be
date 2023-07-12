using PeyulErp.Models;

namespace PeyulErp.Services
{
    public class DefaultDashboardService : IDashboardService
    {
        private readonly IPurchasesService _purchasesService;
        private readonly IExpenseService _expenseService;
        private readonly ITransactionService _salesService;

        public DefaultDashboardService(IPurchasesService purchasesService, IExpenseService expenseService, ITransactionService salesService)
        {
            _purchasesService = purchasesService;
            _expenseService = expenseService;
            _salesService = salesService;
        }

        public async Task<ExpenseSummary> GetExpenseSummary(DateTime startDate, DateTime endDate) => await _expenseService.GetExpenseSummary(startDate, endDate);

        public async Task<SalesSummary> GetSalesSummary(DateTime startDate, DateTime endDate) => await _salesService.GetSalesSummary(startDate, endDate);

        public async Task<PurchaseSummary> GetPurchaseSummary(DateTime startDate, DateTime endDate) => await _purchasesService.GetPurchaseSummary(startDate, endDate);

        //public async Task<Dashboard> GetDashboardSummary(DateTime startDate, DateTime endDate)
        //{
        //    Console.WriteLine($"Getting Dahsboar Summary for dates {startDate} - {endDate}");
        //    var fetchTasks = new List<Task>();
        //    var dashboard = new Dashboard
        //    {
        //        ExpenseSummary = new ExpenseSummary(),
        //        PurchaseSummary = new PurchaseSummary(),
        //        SalesSummary = new SalesSummary()
        //    };

        //    fetchTasks.Add(Task.Run(async () => { dashboard.ExpenseSummary = await GetExpenseSummary(startDate, endDate); }));
        //    fetchTasks.Add(Task.Run(async () => { dashboard.PurchaseSummary = await GetPurchaseSummary(startDate, endDate); }));
        //    fetchTasks.Add(Task.Run(async () => { dashboard.SalesSummary = await GetSalesSummary(startDate, endDate); }));

        //    await Task.WhenAll(fetchTasks);

        //    Console.WriteLine($"Finished fetching dashboard summary: {dashboard}");

        //    return dashboard; 
        //}
        public async Task<Dashboard> GetDashboardSummary(DateTime startDate, DateTime endDate)
        {
            Console.WriteLine($"Getting Dashboard Summary for dates {startDate} - {endDate}");
            var fetchTasks = new List<Task>();
            var dashboard = new Dashboard
            {
                ExpenseSummary = new ExpenseSummary(),
                PurchaseSummary = new PurchaseSummary(),
                SalesSummary = new SalesSummary()
            };

            try
            {
                fetchTasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        dashboard.ExpenseSummary = await GetExpenseSummary(startDate, endDate);
                    }
                    catch (Exception ex)
                    {
                        // Handle exception if necessary
                        Console.WriteLine($"An error occurred while fetching expense summary: {ex.Message}");
                    }
                }));

                fetchTasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        dashboard.PurchaseSummary = await GetPurchaseSummary(startDate, endDate);
                    }
                    catch (Exception ex)
                    {
                        // Handle exception if necessary
                        Console.WriteLine($"An error occurred while fetching purchase summary: {ex.Message}");
                    }
                }));

                fetchTasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        dashboard.SalesSummary = await GetSalesSummary(startDate, endDate);
                    }
                    catch (Exception ex)
                    {
                        // Handle exception if necessary
                        Console.WriteLine($"An error occurred while fetching sales summary: {ex.Message}");
                    }
                }));

                await Task.WhenAll(fetchTasks);

                Console.WriteLine($"Finished fetching dashboard summary: {dashboard}");

                return dashboard;
            }
            catch (Exception ex)
            {
                // Handle any unexpected exception
                Console.WriteLine($"An error occurred while fetching dashboard summary: {ex.Message}");
                // Return a default dashboard or rethrow the exception
                return new Dashboard();
            }
        }
    }
}