using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using UserAuthenticationService.ApplicationService.Models;
using UserAuthenticationService.ApplicationService.Queries;
using UserAuthenticationService.Repository;

namespace UserAuthenticationService.ApplicationService.Handlers
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
    {
        private readonly IUserRepository _userRepository;

        public GetUserByIdQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            
            if (user == null)
                return null;

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                UserType = user.UserType,
                EmailVerified = user.EmailVerified,
                PhoneVerified = user.PhoneVerified,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                IsActive = user.IsActive,
                Roles = user.Roles.Select(r => r.RoleName).ToList(),
                ExternalLogins = user.ExternalLogins.Select(el => new ExternalLoginDto
                {
                    Id = el.Id,
                    Provider = el.Provider,
                    ProviderKey = el.ProviderKey,
                    CreatedAt = el.CreatedAt,
                    LastUsedAt = el.LastUsedAt
                }).ToList()
            };
        }
    }
} 