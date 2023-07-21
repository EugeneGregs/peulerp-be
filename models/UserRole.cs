namespace PeyulErp.Models
{
    public record UserRole : BaseModel
    {
        public string Name { get; init; }
        public List<Priviledge> Priviledges { get; init; }
    }
}