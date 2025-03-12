using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using UserAuthenticationService.ApplicationService.Commands;
using UserAuthenticationService.Domain;
using UserAuthenticationService.Repository;

namespace UserAuthenticationService.ApplicationService.Handlers
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;

        public RegisterUserCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // Check if user with the same email already exists
            if (await _userRepository.ExistsByEmailAsync(request.Email))
            {
                throw new InvalidOperationException($"User with email {request.Email} already exists");
            }

            // Create a new user with a temporary password hash
            var user = new User(
                request.Email,
                "temporary_hash", // Will be replaced with the actual hash
                request.FirstName,
                request.LastName,
                request.PhoneNumber,
                request.UserType);

            // Hash the password
            var passwordHash = _passwordHasher.HashPassword(user, request.Password);
            
            // Update the password hash
            user.UpdatePassword(passwordHash);

            // Add default role based on user type
            var roleName = request.UserType == UserType.Family ? "Family" : 
                           request.UserType == UserType.Caregiver ? "Caregiver" : "Admin";
            
            user.AddRole(new UserRole(user.Id, roleName));

            // Save the user
            await _userRepository.AddAsync(user);

            return user.Id;
        }
    }
} 