using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeyulErp.Services;

namespace PeyulErp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[Controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryManager _inventoryManager;
        public InventoryController(IInventoryManager inventoryManager) {
            _inventoryManager = inventoryManager;
        }

        [HttpGet("/diminishing")]
        public async Task<IActionResult> GetDiminishingProducts()
        {
            var diminishedProducts = await _inventoryManager.GetDiminishedProductsAsync();

            return Ok(diminishedProducts);
        }

        [HttpGet("/stock")]
        public async Task<IActionResult> GetCurrentStock()
        {
            return Ok(await _inventoryManager.GetCurrentStockListAsync());
        }
    }
}