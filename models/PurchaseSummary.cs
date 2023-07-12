namespace PeyulErp.Models
{
    public record class PurchaseSummary
    {
        public double TotalPurchases { get; set; }
        public int PurchaseCount { get; set; }
        public IDictionary<int,double> MonthlyPurchaseTotals { get; set; }
    }
}