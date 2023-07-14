namespace PeyulErp.Contracts{
    public record SaveProductDTO : ProductDTO {
        public Guid CategoryId { get; init; }
        public int ReorderLevel { get; init; }
        public int Quantity { get; init; }
    }
}