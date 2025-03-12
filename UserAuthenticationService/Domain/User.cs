using System;
using System.Collections.Generic;

namespace UserAuthenticationService.Domain
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string PhoneNumber { get; private set; }
        public UserType UserType { get; private set; }
        public bool EmailVerified { get; private set; }
        public bool PhoneVerified { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastLoginAt { get; private set; }
        public bool IsActive { get; private set; }
        public List<UserRole> Roles { get; private set; }
        public List<ExternalLogin> ExternalLogins { get; private set; }

        // Private constructor for EF Core
        private User() 
        {
            Roles = new List<UserRole>();
            ExternalLogins = new List<ExternalLogin>();
        }

        public User(string email, string passwordHash, string firstName, string lastName, 
                   string phoneNumber, UserType userType)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));
            
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));
            
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name cannot be empty", nameof(firstName));
            
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name cannot be empty", nameof(lastName));

            Id = Guid.NewGuid();
            Email = email.ToLowerInvariant();
            PasswordHash = passwordHash;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            UserType = userType;
            EmailVerified = false;
            PhoneVerified = false;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
            Roles = new List<UserRole>();
            ExternalLogins = new List<ExternalLogin>();
        }

        public void VerifyEmail()
        {
            EmailVerified = true;
        }

        public void VerifyPhone()
        {
            PhoneVerified = true;
        }

        public void UpdatePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new ArgumentException("Password hash cannot be empty", nameof(newPasswordHash));
            
            PasswordHash = newPasswordHash;
        }

        public void UpdateProfile(string firstName, string lastName, string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name cannot be empty", nameof(firstName));
            
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name cannot be empty", nameof(lastName));
            
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
        }

        public void RecordLogin()
        {
            LastLoginAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void AddRole(UserRole role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));
            
            if (!Roles.Contains(role))
                Roles.Add(role);
        }

        public void RemoveRole(UserRole role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));
            
            Roles.Remove(role);
        }

        public void AddExternalLogin(ExternalLogin externalLogin)
        {
            if (externalLogin == null)
                throw new ArgumentNullException(nameof(externalLogin));
            
            if (!ExternalLogins.Contains(externalLogin))
                ExternalLogins.Add(externalLogin);
        }

        public void RemoveExternalLogin(ExternalLogin externalLogin)
        {
            if (externalLogin == null)
                throw new ArgumentNullException(nameof(externalLogin));
            
            ExternalLogins.Remove(externalLogin);
        }
    }
} 