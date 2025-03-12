using MediatR;
using UserAuthenticationService.ApplicationService.Models;

namespace UserAuthenticationService.ApplicationService.Commands
{
    public class LoginCommand : IRequest<AuthenticationResult>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string IpAddress { get; set; }
    }
} 