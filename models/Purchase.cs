using PeyulErp.Contracts;

namespace PeyulErp.Models{
    public record Purchase : BaseModel {
        public Guid SupplierId { get; init; }
        public PaymentStatus PaymentStatus { get; init; }
        public int Amount { get; init; }
        public string Description { get; init; }
        public string RefferenceNo { get; init; }
        public DateTime PurchaseDate { get; init; }
        public List<PurchaseProduct> PurchaseProducts { get; init; }
        public PaymentType PaymentType { get; init; }
    }
}