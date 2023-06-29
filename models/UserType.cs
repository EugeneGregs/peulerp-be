namespace PeyulErp.Models{
    public record UserType : BaseModel {
        public IList<Guid> Priviledges { get; init; }
        public string Name { get; init; }
    }
}