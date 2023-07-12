namespace PeyulErp.Models
{
    public record AssetSummary
    {
        public double TotalStock { get; set; }
        public double TotalCash { get; set; }
        public double TotalMobile { get; set; }
        public double TotalReturns { get; set; }
    }
}