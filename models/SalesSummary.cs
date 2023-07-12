namespace PeyulErp.Models
{
    public record SalesSummary
    {
        public double TotalSales { get; set; }
        public double GrossProfit { get; set; }
        public int TransactionCount { get; set; }
        public IDictionary<int,double> MonthlyAggregation { get; set; }
    }
}