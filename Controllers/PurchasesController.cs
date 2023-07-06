//---------------------------------------------------------------------
// <copyright file="TransactionController.cs" company="PEUL Africa">
//   Copyright (c) PEUL Africa. All rights reserved.
// </copyright>
//--------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using PeyulErp.Contracts;
using PeyulErp.Models;
using PeyulErp.Services;

namespace PeyulErp.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class PurchasesController : ControllerBase
    {

        private readonly IPurchasesService _purchasesService;

        public PurchasesController(IPurchasesService purchasesService)
        {
            _purchasesService = purchasesService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPurchasesAsync()
        {
            var purchases = await _purchasesService.GetPurchases();

            return Ok(purchases);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetPurchaseAsync(Guid Id)
        {
            Purchase purchase = await _purchasesService.GetPurchase(Id);

            if (purchase is null)
            {
                return NotFound();
            }

            return Ok(purchase);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePurchaseAsync(Purchase purchase)
        {
            var result = await _purchasesService.UpsertPurchase(purchase);

            return CreatedAtAction(nameof(GetPurchaseAsync), new { Id = result.Id }, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchaseAsync(Guid Id)
        {
            var deleted = await _purchasesService.DeletePurchase(Id);

            if (deleted)
            {
                return Ok();
            }

            return NotFound();
        }
    }
}