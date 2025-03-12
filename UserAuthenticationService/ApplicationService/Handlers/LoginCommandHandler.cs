using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using UserAuthenticationService.ApplicationService.Commands;
using UserAuthenticationService.ApplicationService.Models;
using UserAuthenticationService.ApplicationService.Services;
using UserAuthenticationService.Domain;
using UserAuthenticationService.Repository;

namespace UserAuthenticationService.ApplicationService.Handlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthenticationResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly JwtSettings _jwtSettings;

        public LoginCommandHandler(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IPasswordHasher<User> passwordHasher,
            IJwtService jwtService,
            IOptions<JwtSettings> jwtSettings)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _refreshTokenRepository = refreshTokenRepository ?? throw new ArgumentNullException(nameof(refreshTokenRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
            _jwtSettings = jwtSettings?.Value ?? throw new ArgumentNullException(nameof(jwtSettings));
        }

        public async Task<AuthenticationResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Find user by email
            var user = await _userRepository.GetByEmailAsync(request.Email);
            
            if (user == null)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Message = "Invalid email or password"
                };
            }

            // Verify password
            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(
                user, user.PasswordHash, request.Password);

            if (passwordVerificationResult != PasswordVerificationResult.Success)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Message = "Invalid email or password"
                };
            }

            // Check if user is active
            if (!user.IsActive)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Message = "Account is deactivated"
                };
            }

            // Record login
            user.RecordLogin();
            await _userRepository.UpdateAsync(user);

            // Generate JWT token
            var token = _jwtService.GenerateJwtToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken(request.IpAddress);

            // Save refresh token
            var refreshTokenEntity = new RefreshToken(
                user.Id,
                refreshToken,
                DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
                request.IpAddress);

            await _refreshTokenRepository.AddAsync(refreshTokenEntity);

            // Map user to DTO
            var userDto = new UserDto
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

            return new AuthenticationResult
            {
                Success = true,
                Token = token,
                RefreshToken = refreshToken,
                Expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                User = userDto
            };
        }
    }
} 