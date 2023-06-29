using PeyulErp.Models;

namespace PeyulErp.Services
{
    public interface IPurchasesService
    {
        Task<Purchase> GetPurchase(Guid Id);
        Task<List<Purchase>> GetPurchases();
        Task<Purchase> UpsertPurchase(Purchase purchase);
        Task<bool> DeletePurchase(Guid Id);
    }
}