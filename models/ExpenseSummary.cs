namespace PeyulErp.Models
{
    public record class ExpenseSummary
    {
        public double TotalExpenses { get; set; }
        public double TotalCleaningExpenses { get; set; }
        public double TotalOtherPurchases { get; set; }
        public double TotalUtilities { get; set; }
        public double TotalOther { get; set; }
        public IDictionary<int,double> MonthlyExpenseSummary { get; set; }
    }
}