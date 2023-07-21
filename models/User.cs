namespace PeyulErp.Models{
    public record User : BaseModel {
        public string Name { get; init; }
        public string Email { get; init; }
        public string PhoneNumber { get; init; }
        public string Password { get; init; }
        public UserRole Role { get; init; }
        public UserStatus Status { get; init; }
    }
}