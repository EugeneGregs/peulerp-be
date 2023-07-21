//---------------------------------------------------------------------
// <copyright file="TransactionController.cs" company="PEUL Africa">
//   Copyright (c) PEUL Africa. All rights reserved.
// </copyright>
//--------------------------------------------------------------------

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeyulErp.Contracts;
using PeyulErp.Models;
using PeyulErp.Services;

namespace PeyulErp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class TransactionController : ControllerBase
    {

        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactionsAsync()
        {
            var products = await _transactionService.GetTransactions();

            return Ok(products);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetTransactionAsync(Guid Id)
        {
            Transaction transaction = await _transactionService.GetTransaction(Id);

            if (transaction is null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransactionAsync(Transaction transaction)
        {
            var result = await _transactionService.CreateTransaction(transaction);

            return CreatedAtAction(nameof(GetTransactionAsync), new { Id = result.Id }, result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTransactionAsync(Guid Id)
        {
            var deleted = await _transactionService.DeleteTransaction(Id);

            if (deleted)
            {
                return Ok();
            }

            return NotFound();
        }
    }
}