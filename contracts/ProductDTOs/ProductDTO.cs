namespace PeyulErp.Contracts{
    public record ProductDTO {
        public string Name { get; init; }
        public string BarCode { get; init; }
        public int BuyingPrice { get; init; }
        public int SellingPrice { get; init; }
    }
}