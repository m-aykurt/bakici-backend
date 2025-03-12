using MediatR;
using UserAuthenticationService.ApplicationService.Models;

namespace UserAuthenticationService.ApplicationService.Commands
{
    public class RefreshTokenCommand : IRequest<AuthenticationResult>
    {
        public string RefreshToken { get; set; }
        public string IpAddress { get; set; }
    }
} 