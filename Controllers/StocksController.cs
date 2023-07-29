using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeyulErp.Models;
using PeyulErp.Services;

namespace PeyulErp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class StocksController: ControllerBase
    {
        private readonly IStockService _stockService;

        public StocksController(IStockService stockService)
        {
            _stockService = stockService;            
        }

        [HttpGet]
        public async Task<IActionResult> GetStocksAsync()
        {
            var stocks = await _stockService.GetStocksAsync();

            return Ok(stocks);
        }
        
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetStockAsync(Guid Id)
        {
            Stock stock = await _stockService.GetStockAsync(Id);

            if (stock is null)
            {
                return NotFound();
            }

            return Ok(stock);
        }

        //get stock by product id
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetStockByProductIdAsync(Guid productId)
        {
            Stock stock = await _stockService.GetStockByProductIdAsync(productId);

            if (stock is null)
            {
                return NotFound();
            }

            return Ok(stock);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateStockAsync(Stock stock)
        {
            var result = await _stockService.UpsertStockAsync(stock);

            return CreatedAtAction(nameof(GetStockAsync), new { Id = result.Id }, result);
        }
    }
}