namespace PeyulErp.Models{
    public record ProductPurchase : BaseModel {
        public Guid ProductId { get; init; }
        public Guid PurchaseId { get; init; }
        public int Quantity { get; init; }
        public int BuyingPrice { get; init; }
        public int SellingPrice { get; init; }
    }
}