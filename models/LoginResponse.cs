namespace PeyulErp.Models
{
    public record LoginResponse(string Token, string Email, string Role, bool ResetPassword = false);
}