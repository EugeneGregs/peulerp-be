namespace PeyulErp.Contracts{
    public record PurchaseDTO {
        public int Amount { get; init; }
        public int Status { get; init; }
    }
}