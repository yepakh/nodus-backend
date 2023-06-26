using Nodus.Database.Migrations.gRPC;
using Nodus.NotificaitonService;

namespace Nodus.Jamal.Service.GrpcClients
{
    public class NotificatorGrpcClient
    {
        private readonly Notificator.NotificatorClient _notificatorClient;

        public NotificatorGrpcClient(Notificator.NotificatorClient notificatorClient)
        {
            _notificatorClient = notificatorClient;
        }

        public async Task<SendEmailResponse> SendEmailAsync(string email, string subject, string message)
        {
            var request = new EmailRequest
            {
                Email = email,
                Subject = subject,
                Message = message
            };
            return await _notificatorClient.SendEmailAsync(request);
        }
    }
}
