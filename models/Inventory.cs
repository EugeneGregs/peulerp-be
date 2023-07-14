using PeyulErp.Contracts;

namespace PeyulErp.Models
{
    public record Inventory : GetProductDTO
    {
        public int Quantity { get; set; }
        public int ReorderLevel { get; set; }
        public Guid ProductId { get; set; }
    }
}