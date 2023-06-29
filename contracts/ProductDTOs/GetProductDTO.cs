namespace PeyulErp.Contracts{
    public record GetProductDTO : ProductDTO {
        public Guid Id { get; init; }
        public GetProductCategoryDTO ProductCategory { get; init; }
        public DateTime CreatedDate { get; init; }
        public DateTime UpdatedDate { get; init; }
    }
}