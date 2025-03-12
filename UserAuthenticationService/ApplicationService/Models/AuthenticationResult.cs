using System;

namespace UserAuthenticationService.ApplicationService.Models
{
    public class AuthenticationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? Expiration { get; set; }
        public UserDto User { get; set; }
    }
} 