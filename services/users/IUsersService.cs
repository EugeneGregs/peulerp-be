using PeyulErp.Models;

namespace PeyulErp.Services
{
    public interface IUsersService
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByIdAsync(Guid id);
        Task<User> CreateAsync(User user, bool isDefaultUser = false);
        Task<User> UpdateAsync(User user);
        Task<bool> DeleteAsync(Guid id);
        Task<IList<User>> GetAllAsync();
        Task UpdateUserStatusAsync(Guid Id, UserStatus status);
        Task UpdateUserPassWordAsync(Guid Id, string password, bool forceReset = false);
        Task<bool> ResetUserPasswordAsync(User user);
    }
}