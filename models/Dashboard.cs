using PeyulErp.Contracts;

namespace PeyulErp.Models
{
    public record Dashboard
    {
        public SalesSummary SalesSummary { get; set; }
        public PurchaseSummary PurchaseSummary { get; set; }
        public ExpenseSummary ExpenseSummary { get; set; }
        public AssetSummary AssetSummary { get; set; }
    }
}