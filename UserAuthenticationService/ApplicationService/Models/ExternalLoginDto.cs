using System;

namespace UserAuthenticationService.ApplicationService.Models
{
    public class ExternalLoginDto
    {
        public Guid Id { get; set; }
        public string Provider { get; set; }
        public string ProviderKey { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUsedAt { get; set; }
    }
} 