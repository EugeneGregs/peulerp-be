namespace PeyulErp.Models{
    public record BaseModel{
        public Guid Id { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}