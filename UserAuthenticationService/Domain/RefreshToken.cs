using System;

namespace UserAuthenticationService.Domain
{
    public class RefreshToken
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string Token { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string CreatedByIp { get; private set; }
        public DateTime? RevokedAt { get; private set; }
        public string RevokedByIp { get; private set; }
        public string ReplacedByToken { get; private set; }
        public string ReasonRevoked { get; private set; }

        // Navigation property
        public User User { get; private set; }

        // Private constructor for EF Core
        private RefreshToken() { }

        public RefreshToken(Guid userId, string token, DateTime expiresAt, string createdByIp)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty", nameof(userId));
            
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token cannot be empty", nameof(token));
            
            if (expiresAt <= DateTime.UtcNow)
                throw new ArgumentException("Expiration date must be in the future", nameof(expiresAt));
            
            Id = Guid.NewGuid();
            UserId = userId;
            Token = token;
            ExpiresAt = expiresAt;
            CreatedAt = DateTime.UtcNow;
            CreatedByIp = createdByIp;
        }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsRevoked => RevokedAt != null;
        public bool IsActive => !IsRevoked && !IsExpired;

        public void Revoke(string ipAddress, string reason = null, string replacementToken = null)
        {
            RevokedAt = DateTime.UtcNow;
            RevokedByIp = ipAddress;
            ReasonRevoked = reason;
            ReplacedByToken = replacementToken;
        }
    }
} 