using MediatR;

namespace UserAuthenticationService.ApplicationService.Commands
{
    public class ForgotPasswordCommand : IRequest<bool>
    {
        public string Email { get; set; }
        public string ResetUrl { get; set; }
    }
} 