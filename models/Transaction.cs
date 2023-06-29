namespace PeyulErp.Models{
    public record Transaction : BaseModel {
        public IList<CartItem> CartItems { get; init; }
        public PaymentType PaymentType { get; init; }
        public string OrderNumber { get; init; }
        public Guid TellerId { get; init; }
        public double Discount { get; init; }
        public Guid CustomerId { get; init; }
        public double TotalCost { get; init; }
        public double TotalMargin { get; init; }
    }
}