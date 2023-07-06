using PeyulErp.Contracts;

namespace PeyulErp.Models
{
    public record StockDTO : Stock
    {
        public GetProductDTO Product { get; set; }
    }
}