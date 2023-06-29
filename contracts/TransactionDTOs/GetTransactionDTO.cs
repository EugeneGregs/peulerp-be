namespace PeyulErp.Contracts {
    public record GetTransactionDTO : TransactionDTO {
        public Guid Id { get; init; }
        public IList<ProductDTO> Products { get; init; }
    }
}