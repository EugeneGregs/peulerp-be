namespace PeyulErp.Models
{
    public record Expense : BaseModel
    {
        public string Name { get; init; }
        public string Description { get; set; }
        public PaymentType PaymentType { get; set; }
        public double Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public ExpenseType ExpenseType { get; set; }
    }
}