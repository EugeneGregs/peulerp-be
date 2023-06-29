namespace PeyulErp.Models
{
    public record PurchaseProduct
    {
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
        public int ReorderLevel { get; init; }
    }
}