namespace PeyulErp.Models{
    public record Supplier : BaseModel {
        public string Name { get; init; }
        public string Address { get; init; }
        public string Phone { get; init; }
        public string Email { get; init; }
        public string Description { get; init; }
    }
}