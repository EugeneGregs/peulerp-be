using PeyulErp.Models;

namespace PeyulErp.Services
{
    public interface ISupplierService
    {
        Task<List<Supplier>> GetSuppliersAsync();
        Task<Supplier> GetSupplierAsync(Guid id);
        Task<Supplier> CreateSupplierAsync(Supplier supplier);
        Task<Supplier> UpdateSupplierAsync(Supplier supplier);
        Task<bool> DeleteSupplierAsync(Guid id);
    }
}