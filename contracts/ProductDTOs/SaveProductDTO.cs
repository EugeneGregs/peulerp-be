namespace PeyulErp.Contracts{
    public record SaveProductDTO : ProductDTO {
        public Guid CategoryId { get; init; }
    }
}