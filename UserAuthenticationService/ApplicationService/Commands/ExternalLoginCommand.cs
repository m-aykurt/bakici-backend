using MediatR;
using UserAuthenticationService.ApplicationService.Models;

namespace UserAuthenticationService.ApplicationService.Commands
{
    public class ExternalLoginCommand : IRequest<AuthenticationResult>
    {
        public string Provider { get; set; }
        public string ProviderKey { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IpAddress { get; set; }
    }
} 