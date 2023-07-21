using PeyulErp.Model;

namespace PeyulErp.Services
{
    public interface IMailingService
    {
        Task SendMailAsync(Email email);
    }
}