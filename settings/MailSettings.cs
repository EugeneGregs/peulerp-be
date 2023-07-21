namespace PeyulErp.Settings
{
    public class MailSettings
    {
        public string SMTPServer { get; set; }
        public string SMTPName { get; set; }
        public string SMTPUserName { get; set; }
        public string SMTPPassword { get; set; }
        public int SMTPTLSPort { get; set; }
        public int SMTPSSLPort { get; set; }
    }
}