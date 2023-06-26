using Grpc.Core;
using Nodus.NotificaitonService;
using Nodus.NotificaitonService.Services;

namespace Nodus.NotificaitonService.GrpcServices
{
    public class NotificationGrpcService : Notificator.NotificatorBase
    {
        private readonly ILogger<NotificationGrpcService> _logger;
        private readonly EmailService _emailService;
        public NotificationGrpcService(ILogger<NotificationGrpcService> logger, EmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
        }

        public override Task<SendEmailResponse> SendEmail(EmailRequest request, ServerCallContext context)
        {
            return _emailService.SendEmail(request);
        }
    }
}