using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserAuthenticationService.Domain;

namespace UserAuthenticationService.Repository.EntityFramework
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AuthDbContext _dbContext;

        public RefreshTokenRepository(AuthDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<RefreshToken> GetByIdAsync(Guid id)
        {
            return await _dbContext.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Id == id);
        }

        public async Task<RefreshToken> GetByTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token cannot be empty", nameof(token));

            return await _dbContext.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task<IEnumerable<RefreshToken>> GetByUserIdAsync(Guid userId)
        {
            return await _dbContext.RefreshTokens
                .Where(rt => rt.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<RefreshToken>> GetActiveByUserIdAsync(Guid userId)
        {
            var now = DateTime.UtcNow;
            return await _dbContext.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.RevokedAt == null && rt.ExpiresAt > now)
                .ToListAsync();
        }

        public async Task AddAsync(RefreshToken refreshToken)
        {
            if (refreshToken == null)
                throw new ArgumentNullException(nameof(refreshToken));

            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(RefreshToken refreshToken)
        {
            if (refreshToken == null)
                throw new ArgumentNullException(nameof(refreshToken));

            _dbContext.RefreshTokens.Update(refreshToken);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var refreshToken = await _dbContext.RefreshTokens.FindAsync(id);
            if (refreshToken != null)
            {
                _dbContext.RefreshTokens.Remove(refreshToken);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteAllByUserIdAsync(Guid userId)
        {
            var refreshTokens = await _dbContext.RefreshTokens
                .Where(rt => rt.UserId == userId)
                .ToListAsync();

            if (refreshTokens.Any())
            {
                _dbContext.RefreshTokens.RemoveRange(refreshTokens);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
} 