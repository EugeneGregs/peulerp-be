using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using PeyulErp.Model;
using PeyulErp.Settings;

namespace PeyulErp.Services
{
    public class MailKitMailingService: IMailingService
    {
        private readonly MailSettings _mailSettings;

        public MailKitMailingService(IOptions<MailSettings> mailSettingsOption)
        {
            _mailSettings = mailSettingsOption.Value;
        }

        public async Task SendMailAsync(Email email){
            Console.WriteLine($"mail Server: {_mailSettings.SMTPServer}, User Name: {_mailSettings.SMTPUserName}, Password: {_mailSettings.SMTPPassword}");
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_mailSettings.SMTPName, _mailSettings.SMTPUserName));
                message.To.Add(new MailboxAddress(email.ReciverName, email.ReciverEmail));
                message.Subject = email.Subject;

                message.Body = new TextPart("plain")
                {
                    Text = email.Body
                };

                using (var client = new SmtpClient())
                {
                    client.Connect(_mailSettings.SMTPServer, _mailSettings.SMTPTLSPort, false);

                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate(_mailSettings.SMTPUserName, _mailSettings.SMTPPassword);

                    await client.SendAsync(message);
                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                throw new Exception(message);
            }
        }
    }
}