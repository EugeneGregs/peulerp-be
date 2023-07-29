
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeyulErp.Contracts;
using PeyulErp.Services;
using PeyulErp.Utility;

namespace PeyulErp.Controllers{

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase {

        private readonly IProductsService _productService;

        public ProductsController(IProductsService prodcutservice)
        {
            _productService = prodcutservice;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductsAsync(){
            var products = await _productService.GetProductsAsync();

            return Ok(products);
        }
        
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetProductAsync(Guid Id){
            GetProductDTO product = await _productService.GetProductAsync(Id);

            if(product is null){
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> SaveProductAsync(SaveProductDTO product){
            var result = await _productService.CreateProductAsync(product);

            return CreatedAtAction(nameof(GetProductAsync), new { Id = result.Id }, result);
        }
        
        [HttpPost("colection")]
        public async Task<IActionResult> SaveProductsAsync(IList<SaveProductDTO> products){
            var result = new List<GetProductDTO>();

            foreach(var productDto in products){
                result.Add(await _productService.CreateProductAsync(productDto));
            }

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductAsync(SaveProductDTO productDTO, Guid Id){
            var updated = await _productService.UpdateProductAsync(productDTO, Id);

            if(updated){
                return NoContent();
            }

            return NotFound();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = IdentityHelper.AdminUserPolicyName)]
        public async Task<IActionResult> DeleteProductAsync(Guid Id){
            var deleted = await _productService.DeleteProductAsync(Id);

            if(deleted){
                return Ok();
            }

            return NotFound();
        }
    }
}