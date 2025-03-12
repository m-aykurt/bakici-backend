using System;
using System.Collections.Generic;

namespace UserAuthenticationService.ApiContract.Responses
{
    public class AuthenticationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? Expiration { get; set; }
        public UserResponse User { get; set; }
    }

    public class UserResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string UserType { get; set; }
        public bool EmailVerified { get; set; }
        public bool PhoneVerified { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; }
        public List<string> Roles { get; set; }
    }
} 