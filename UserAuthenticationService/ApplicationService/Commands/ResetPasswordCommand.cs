using MediatR;

namespace UserAuthenticationService.ApplicationService.Commands
{
    public class ResetPasswordCommand : IRequest<bool>
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
} 