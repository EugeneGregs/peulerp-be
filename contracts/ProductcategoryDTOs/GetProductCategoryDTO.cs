namespace PeyulErp.Contracts{
    public record GetProductCategoryDTO : ProductCategoryDTO {
        public Guid Id { get; init; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}