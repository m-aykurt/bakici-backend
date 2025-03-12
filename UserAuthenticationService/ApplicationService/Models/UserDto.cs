using System;
using System.Collections.Generic;
using UserAuthenticationService.Domain;

namespace UserAuthenticationService.ApplicationService.Models
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public UserType UserType { get; set; }
        public bool EmailVerified { get; set; }
        public bool PhoneVerified { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; }
        public List<string> Roles { get; set; }
        public List<ExternalLoginDto> ExternalLogins { get; set; }
    }
} 