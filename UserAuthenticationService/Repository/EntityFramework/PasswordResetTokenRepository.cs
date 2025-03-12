using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserAuthenticationService.Domain;

namespace UserAuthenticationService.Repository.EntityFramework
{
    public class PasswordResetTokenRepository : IPasswordResetTokenRepository
    {
        private readonly AuthDbContext _dbContext;

        public PasswordResetTokenRepository(AuthDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<PasswordResetToken> GetByIdAsync(Guid id)
        {
            return await _dbContext.PasswordResetTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<PasswordResetToken> GetByTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token cannot be empty", nameof(token));

            return await _dbContext.PasswordResetTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Token == token);
        }

        public async Task<PasswordResetToken> GetValidTokenByUserIdAsync(Guid userId)
        {
            var now = DateTime.UtcNow;
            return await _dbContext.PasswordResetTokens
                .Include(t => t.User)
                .Where(t => t.UserId == userId && !t.IsUsed && t.ExpiresAt > now)
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(PasswordResetToken passwordResetToken)
        {
            if (passwordResetToken == null)
                throw new ArgumentNullException(nameof(passwordResetToken));

            await _dbContext.PasswordResetTokens.AddAsync(passwordResetToken);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(PasswordResetToken passwordResetToken)
        {
            if (passwordResetToken == null)
                throw new ArgumentNullException(nameof(passwordResetToken));

            _dbContext.PasswordResetTokens.Update(passwordResetToken);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var passwordResetToken = await _dbContext.PasswordResetTokens.FindAsync(id);
            if (passwordResetToken != null)
            {
                _dbContext.PasswordResetTokens.Remove(passwordResetToken);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteAllByUserIdAsync(Guid userId)
        {
            var tokens = await _dbContext.PasswordResetTokens
                .Where(t => t.UserId == userId)
                .ToListAsync();

            if (tokens.Any())
            {
                _dbContext.PasswordResetTokens.RemoveRange(tokens);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteExpiredTokensAsync()
        {
            var now = DateTime.UtcNow;
            var expiredTokens = await _dbContext.PasswordResetTokens
                .Where(t => t.ExpiresAt <= now || t.IsUsed)
                .ToListAsync();

            if (expiredTokens.Any())
            {
                _dbContext.PasswordResetTokens.RemoveRange(expiredTokens);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
} 