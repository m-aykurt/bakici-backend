using System;
using MediatR;
using UserAuthenticationService.ApplicationService.Models;

namespace UserAuthenticationService.ApplicationService.Queries
{
    public class GetUserByIdQuery : IRequest<UserDto>
    {
        public Guid UserId { get; set; }
    }
} 