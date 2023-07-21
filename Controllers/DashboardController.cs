using Microsoft.AspNetCore.Mvc;
using PeyulErp.Contracts;
using PeyulErp.Models;
using PeyulErp.Services;

namespace PeyulErp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly ISupplierService _supplierService;
        private readonly IPurchasesService _purchasesService;
        private readonly IInventoryManager _inventoryManager;
        private readonly IExpenseService _expenseService;
        private readonly ITransactionService _transactionService;
        private readonly IDashboardService _dashboardService;

        public DashboardController(
            ISupplierService supplierService,
            IPurchasesService purchasesService,
            IInventoryManager inventoryManager,
            IExpenseService expenseService,
            ITransactionService transactionService,
            IDashboardService dashboardService)
        {
            _supplierService = supplierService;
            _purchasesService = purchasesService;
            _inventoryManager = inventoryManager;
            _expenseService = expenseService;
            _transactionService = transactionService;
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashBoardAsync(ReportFilter filter = null)
        {
            if(filter is null)
            {
                return Ok(await _dashboardService.GetDashboardSummaryAsync(DateTime.UtcNow, DateTime.UtcNow));
            }
            else
            {
                GetReportDates(out var startDate, out var endDate, filter);
                return Ok(await _dashboardService.GetDashboardSummaryAsync(startDate, endDate));
            }
        }

        private void GetReportDates(out DateTime startDate, out DateTime endDate, ReportFilter filter)
        {
            switch(filter.FilterType)
            {
                case ReportFilterType.Day:
                    startDate = DateTime.UtcNow.AddDays(-1);
                    endDate = DateTime.UtcNow;
                    break;
                case ReportFilterType.Week:
                    startDate = DateTime.UtcNow.AddDays(-7);
                    endDate = DateTime.UtcNow;
                    break;
                case ReportFilterType.Month:
                    startDate = DateTime.UtcNow.AddDays(-30);
                    endDate = DateTime.UtcNow;
                    break;
                case ReportFilterType.Year:
                    startDate = DateTime.UtcNow.AddDays(-365);
                    endDate = DateTime.Now;
                    break;
                case ReportFilterType.Costom:
                    startDate = filter.StartDate;
                    endDate = filter.EndDate;
                    break;
                default:
                    startDate = DateTime.UtcNow;
                    endDate = DateTime.UtcNow;
                    break;
            }
        }
        
    }
}