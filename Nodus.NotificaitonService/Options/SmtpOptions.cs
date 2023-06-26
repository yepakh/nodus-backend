namespace Nodus.NotificaitonService.Options
{
    public class SmtpOptions
    {
        public string Email { get; set; }
        public string AppPassword { get; set; }
        public int Port { get; set; }
        public string Address { get; set; }
        public bool EnableSSL { get; set; }
    }
}
