namespace PeyulErp.Settings
{
    public class JwtSettings
    {
        public string Issuer { get; set; }
        public string Key { get; set; }
        public string Audience { get; set; }
        public int ExpirationInHours { get; set; }
    }
}