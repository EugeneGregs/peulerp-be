namespace PeyulErp.Models
{
    public record UserPassword : BaseModel
    {
        public Guid UserId { get; set; }
        public string CPassword { get; set; }
        public string LPassword { get; set; }
        public int FailedAttempts { get; set; }
        public bool ForceReset { get; set; }
    }
}