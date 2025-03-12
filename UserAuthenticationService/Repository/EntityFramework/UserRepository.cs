using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserAuthenticationService.Domain;

namespace UserAuthenticationService.Repository.EntityFramework
{
    public class UserRepository : IUserRepository
    {
        private readonly AuthDbContext _dbContext;

        public UserRepository(AuthDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            return await _dbContext.Users
                .Include(u => u.Roles)
                .Include(u => u.ExternalLogins)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));

            return await _dbContext.Users
                .Include(u => u.Roles)
                .Include(u => u.ExternalLogins)
                .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant());
        }

        public async Task<User> GetByExternalLoginAsync(string provider, string providerKey)
        {
            if (string.IsNullOrWhiteSpace(provider))
                throw new ArgumentException("Provider cannot be empty", nameof(provider));

            if (string.IsNullOrWhiteSpace(providerKey))
                throw new ArgumentException("Provider key cannot be empty", nameof(providerKey));

            return await _dbContext.Users
                .Include(u => u.Roles)
                .Include(u => u.ExternalLogins)
                .FirstOrDefaultAsync(u => u.ExternalLogins.Any(
                    el => el.Provider == provider && el.ProviderKey == providerKey));
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _dbContext.Users
                .Include(u => u.Roles)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetByUserTypeAsync(UserType userType)
        {
            return await _dbContext.Users
                .Include(u => u.Roles)
                .Where(u => u.UserType == userType)
                .ToListAsync();
        }

        public async Task AddAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user != null)
            {
                _dbContext.Users.Remove(user);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));

            return await _dbContext.Users.AnyAsync(u => u.Email == email.ToLowerInvariant());
        }
    }
} 