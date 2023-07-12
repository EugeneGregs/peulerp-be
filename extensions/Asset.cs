namespace PeyulErp.Models
{
    public record class Asset : BaseModel
    {
        public AssetType Type { get; set; }
        public double Amount { get; set; }
    }
}