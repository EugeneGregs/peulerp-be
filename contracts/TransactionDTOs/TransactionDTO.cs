namespace PeyulErp.Contracts{
    public record TransactionDTO {
        public Guid PaymentType { get; init; }
        public int Amount { get; init; }
    }
}