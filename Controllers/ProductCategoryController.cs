using Microsoft.AspNetCore.Mvc;
using PeyulErp.Contracts;
using PeyulErp.Extensions;
using PeyulErp.Services;

namespace PeyulErp.Controllers {

    [ApiController]
    [Route("[controller]")]
    public class ProductCategoryController : ControllerBase {

        private readonly IProductCategoryService _productCategoryService;
        public ProductCategoryController(IProductCategoryService productCategoryService)
        {
            _productCategoryService = productCategoryService;
        }

        [HttpGet]
        public IActionResult GetProductCategories() => Ok(_productCategoryService.GetProductCategories());
        
        [HttpGet("{Id}")]
        public IActionResult GetProductCategory(Guid Id){
            var productCategory = _productCategoryService.GetProductCategory(Id).AsDTO();

            if( productCategory is null){
                return NotFound();
            }

            return Ok(productCategory);
        }

        [HttpPost]
        public IActionResult CreateProductCategory(ProductCategoryDTO productcategory){
            var createdCategory = _productCategoryService.CreateProductCategory(productcategory);

            return CreatedAtAction(nameof(GetProductCategory), new { Id = createdCategory.Id }, createdCategory);
        }

        [HttpPut("{Id}")]
        public IActionResult UpdateProductCategory(ProductCategoryDTO productcategory, Guid Id){
            var updated = _productCategoryService.UpdateProductCategory(productcategory, Id);

            if(updated is false){
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{Id}")]
        public IActionResult DeleteProductCategory(Guid Id){
            var deleted = _productCategoryService.DeleteProductCategory(Id);

            if(deleted is false){
                return NotFound();
            }

            return NoContent();
        }
    }
}