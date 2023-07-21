using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeyulErp.Contracts;
using PeyulErp.Extensions;
using PeyulErp.Models;
using PeyulErp.Services;

namespace PeyulErp.Controllers {

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ProductCategoryController : ControllerBase {

        private readonly IProductCategoryService _productCategoryService;
        public ProductCategoryController(IProductCategoryService productCategoryService)
        {
            _productCategoryService = productCategoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductCategories() => Ok( await _productCategoryService.GetProductCategoriesAsync());
        
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetProductCategory(Guid Id){
            var productCategory = (await _productCategoryService.GetProductCategoryAsync(Id)).AsDTO();

            if( productCategory is null){
                return NotFound();
            }

            return Ok(productCategory);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProductCategory(ProductCategoryDTO productcategory){
            var createdCategory = await _productCategoryService.CreateProductCategoryAsync(productcategory);

            return CreatedAtAction(nameof(GetProductCategory), new { Id = createdCategory.Id }, createdCategory);
        }

        [HttpPost("collection")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SaveCategories(IList<ProductCategoryDTO> productcategories)
        {
            var tasks = new List<Task<GetProductCategoryDTO>>();

            foreach (var category in productcategories)
            {
               tasks.Add(_productCategoryService.CreateProductCategoryAsync(category));
            }

            await Task.WhenAll(tasks);

            return Ok("");
        }

        [HttpPut("{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProductCategory(ProductCategoryDTO productcategory, Guid Id){
            var updated = await _productCategoryService.UpdateProductCategoryAsync(productcategory, Id);

            if(updated is false){
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProductCategory(Guid Id){
            var deleted = await _productCategoryService.DeleteProductCategoryAsync(Id);

            if(deleted is false){
                return NotFound();
            }

            return NoContent();
        }
    }
}