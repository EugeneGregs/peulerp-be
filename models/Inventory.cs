using PeyulErp.Contracts;

namespace PeyulErp.Models
{
    public record Inventory : GetProductDTO
    {
        public int Quantity { get; set; }
    }
}