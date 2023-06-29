namespace PeyulErp.Models{
    public record ProductSale : BaseModel {
        public Guid ProductId { get; init; }
        public Guid TransctionId { get; init; }
        public int Quantity { get; init; }
    }
}