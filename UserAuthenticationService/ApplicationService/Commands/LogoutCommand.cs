using System;
using MediatR;

namespace UserAuthenticationService.ApplicationService.Commands
{
    public class LogoutCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public string RefreshToken { get; set; }
        public string IpAddress { get; set; }
    }
} 