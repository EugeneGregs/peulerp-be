using Microsoft.AspNetCore.Mvc;
using PeyulErp.Models;
using PeyulErp.Services;

namespace PeyulErp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseService _expenseService;
        public ExpenseController(IExpenseService expenseService) {
            _expenseService = expenseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetExpenses() => Ok(await _expenseService.GetExpensesAsync());

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetExpense(Guid Id)
        {
            var expense = await _expenseService.GetExpenseAsync(Id);

            if(expense is null)
            {
                return NotFound();
            }

            return Ok(expense);
        }

        [HttpPost]
        public async Task<IActionResult> CreateExpense(Expense expense)
        {
            var createdExpense = await _expenseService.CreateExpenseAsync(expense);

            return CreatedAtAction(nameof(GetExpense), new { Id = createdExpense.Id }, createdExpense);
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateExpense(Expense expense, Guid Id)
        {
            var updated = await _expenseService.UpdateExpenseAsync(expense with { Id = Id });

            if(updated is null)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteExpense(Guid Id)
        {
            var deleted = await _expenseService.DeleteExpenseAsync(Id);

            if(deleted is false)
            {
                return NotFound();
            }

            return NoContent();
        }
        
    }
}