using PeyulErp.Models;

namespace PeyulErp.Services
{
    public interface IPasswordService
    {
        Task<UserPassword> GetByUserId(Guid userId);
        Task IncrementFailedAttempts(Guid UserId);
        Task Upsert(User user, bool forceReset = false);
        Task<bool> DeletePassword(Guid userId);
        Task<bool> ResetPasswordAttempts(Guid userId);
    }
}