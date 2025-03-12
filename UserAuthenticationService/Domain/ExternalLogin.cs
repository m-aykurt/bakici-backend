using System;

namespace UserAuthenticationService.Domain
{
    public class ExternalLogin
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string Provider { get; private set; }
        public string ProviderKey { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastUsedAt { get; private set; }

        // Navigation property
        public User User { get; private set; }

        // Private constructor for EF Core
        private ExternalLogin() { }

        public ExternalLogin(Guid userId, string provider, string providerKey)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty", nameof(userId));
            
            if (string.IsNullOrWhiteSpace(provider))
                throw new ArgumentException("Provider cannot be empty", nameof(provider));
            
            if (string.IsNullOrWhiteSpace(providerKey))
                throw new ArgumentException("Provider key cannot be empty", nameof(providerKey));
            
            Id = Guid.NewGuid();
            UserId = userId;
            Provider = provider;
            ProviderKey = providerKey;
            CreatedAt = DateTime.UtcNow;
        }

        public void RecordUsage()
        {
            LastUsedAt = DateTime.UtcNow;
        }
    }
} 