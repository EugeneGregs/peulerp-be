namespace PeyulErp.Models{
    public record Product : BaseModel{
        public Guid CategoryId { get; init; }
        public string Name { get; init; }
        public string BarCode { get; init; }
        public int BuyingPrice { get; init; }
        public int SellingPrice { get; init; }
    }
}