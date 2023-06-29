namespace PeyulErp.Contracts{
    public record SaveTransactionDTO : TransactionDTO {
        public IList<Guid> ProductIds { get; init; }
    }
}