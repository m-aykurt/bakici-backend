using System;

namespace UserAuthenticationService.Domain
{
    public class UserRole
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string RoleName { get; private set; }
        public DateTime CreatedAt { get; private set; }

        // Navigation property
        public User User { get; private set; }

        // Private constructor for EF Core
        private UserRole() { }

        public UserRole(Guid userId, string roleName)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty", nameof(userId));
            
            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentException("Role name cannot be empty", nameof(roleName));
            
            Id = Guid.NewGuid();
            UserId = userId;
            RoleName = roleName;
            CreatedAt = DateTime.UtcNow;
        }
    }
} 