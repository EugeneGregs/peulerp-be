namespace PeyulErp.Models{
    public record Stock : BaseModel {
        public Guid ProductId { get; init; }
        public int Quantity { get; set; }
        public int ReorderLevel { get; set; }
    }
}