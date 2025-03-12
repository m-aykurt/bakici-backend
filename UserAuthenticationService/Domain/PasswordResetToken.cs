using System;

namespace UserAuthenticationService.Domain
{
    public class PasswordResetToken
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string Token { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public bool IsUsed { get; private set; }
        public DateTime? UsedAt { get; private set; }

        // Navigation property
        public User User { get; private set; }

        // Private constructor for EF Core
        private PasswordResetToken() { }

        public PasswordResetToken(Guid userId, string token, TimeSpan expiresIn)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty", nameof(userId));
            
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token cannot be empty", nameof(token));
            
            Id = Guid.NewGuid();
            UserId = userId;
            Token = token;
            CreatedAt = DateTime.UtcNow;
            ExpiresAt = CreatedAt.Add(expiresIn);
            IsUsed = false;
        }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsValid => !IsUsed && !IsExpired;

        public void MarkAsUsed()
        {
            if (IsUsed)
                throw new InvalidOperationException("Token has already been used");
            
            if (IsExpired)
                throw new InvalidOperationException("Token has expired");
            
            IsUsed = true;
            UsedAt = DateTime.UtcNow;
        }
    }
} 