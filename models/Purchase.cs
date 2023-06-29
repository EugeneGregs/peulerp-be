using PeyulErp.Contracts;

namespace PeyulErp.Models{
    public record Purchase : BaseModel {
        public Guid SupplierId { get; init; }
        public PaymentType PaymentType { get; init; }
        public int Amount { get; init; }
        public int Status { get; init; }
        public List<Stock> StockList { get; init; }
    }
}