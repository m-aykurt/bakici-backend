using System;
using MediatR;
using UserAuthenticationService.Domain;

namespace UserAuthenticationService.ApplicationService.Commands
{
    public class RegisterUserCommand : IRequest<Guid>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public UserType UserType { get; set; }
    }
} 