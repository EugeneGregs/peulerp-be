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

        public async Task<ExpenseSummary> GetExpenseSummaryAsync(DateTime startDate, DateTime endDate) => await _expenseService.GetExpenseSummary(startDate, endDate);

        public async Task<SalesSummary> GetSalesSummaryAsync(DateTime startDate, DateTime endDate) => await _salesService.GetSalesSummary(startDate, endDate);

        public async Task<PurchaseSummary> GetPurchaseSummaryAsync(DateTime startDate, DateTime endDate) => await _purchasesService.GetPurchaseSummary(startDate, endDate);

        public async Task<Dashboard> GetDashboardSummaryAsync(DateTime startDate, DateTime endDate)
        {
            var fetchTasks = new List<Task>();
            var dashboard = new Dashboard
            {
                ExpenseSummary = new ExpenseSummary(),
                PurchaseSummary = new PurchaseSummary(),
                SalesSummary = new SalesSummary(),
                CashSummary = new CashSummary(),
            };

            try
            {
                fetchTasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        dashboard.ExpenseSummary = await GetExpenseSummaryAsync(startDate, endDate);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred while fetching expense summary: {ex.Message}");
                    }
                }));

                fetchTasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        dashboard.CashSummary = await GetCashSummaryAsync();
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine($"An error occurred while fetching cash summary: {ex.Message}");
                    }
                }));

                fetchTasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        dashboard.PurchaseSummary = await GetPurchaseSummaryAsync(startDate, endDate);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred while fetching purchase summary: {ex.Message}");
                    }
                }));

                fetchTasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        dashboard.SalesSummary = await GetSalesSummaryAsync(startDate, endDate);
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

        public async Task<CashSummary> GetCashSummaryAsync()
        {
            var cashSummary = new CashSummary { CashInMobile = 0, CashInShop = 0 };
            var purchasesSumary = new Dictionary<string, double>();
            var salesSummary = new Dictionary<string, double>();
            var expensesSummary = new Dictionary<string, double>();
            var tasks = new List<Task>();

            try
            {
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        purchasesSumary = await GetPurchasesSummary();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred while fetching purchases cash summary: {ex.Message}");
                    }
                }));

                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        salesSummary = await GetSalesSummary();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred while fetching sales cash summary: {ex.Message}");
                    }
                }));

                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        expensesSummary = await GetExpensesSummary();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred while fetching expenses cash summary: {ex.Message}");
                    }
                }));

                await Task.WhenAll(tasks);

                cashSummary.CashInMobile =  salesSummary["Mobile"];
                cashSummary.CashInShop = salesSummary["Cash"] - (expensesSummary["Cash"] + purchasesSumary["Cash"]);

                return cashSummary;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching cash summary: {ex.Message}");
                return new CashSummary();
            }
        }

        private async Task<Dictionary<string,double>> GetPurchasesSummary()
        {
            var cashPurchases = await _purchasesService.GetByPaymentType(PaymentType.Cash);
            var amountSummary = new Dictionary<string, double>
            {
                { "Cash", cashPurchases.Sum(p => p.Amount) }
            };

            return amountSummary;
        }

        private async Task<Dictionary<string, double>> GetSalesSummary()
        {
            var cashSales = await _salesService.GetByPaymentType(PaymentType.Cash);
            var mobileSales = await _salesService.GetByPaymentType(PaymentType.Mobile);
            var amountSummary = new Dictionary<string, double>
            {
                { "Cash", cashSales.Sum(p => p.TotalCost) },
                { "Mobile", mobileSales.Sum(p => p.TotalCost) }
            };

            return amountSummary;
        }

        private async Task<Dictionary<string, double>> GetExpensesSummary()
        {
            var cashExpenses = await _expenseService.GetByPaymentType(PaymentType.Cash);
            var amountSummary = new Dictionary<string, double>
            {
                { "Cash", cashExpenses.Sum(p => p.Amount) }
            };

            return amountSummary;
        }
    }
}