using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeyulErp.Models;
using PeyulErp.Services;
using System.Data;

namespace PeyulErp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _supplierService;
        public SuppliersController(ISupplierService supplierService) {
            _supplierService = supplierService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSuppliers() => Ok(await _supplierService.GetSuppliersAsync());

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetSupplier(Guid Id)
        {
            var supplier = await _supplierService.GetSupplierAsync(Id);

            if(supplier is null)
            {
                return NotFound();
            }

            return Ok(supplier);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSupplier(Supplier supplier)
        {
            var createdSupplier = await _supplierService.CreateSupplierAsync(supplier);

            return CreatedAtAction(nameof(GetSupplier), new { Id = createdSupplier.Id }, createdSupplier);
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateSupplier(Supplier supplier, Guid Id)
        {
            var updated = await _supplierService.UpdateSupplierAsync(supplier);

            if(updated is null)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSupplier(Guid Id)
        {
            var deleted = await _supplierService.DeleteSupplierAsync(Id);

            if(deleted is false)
            {
                return NotFound();
            }

            return NoContent();
        }        
    }
}