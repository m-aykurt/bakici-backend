using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserAuthenticationService.Domain;

namespace UserAuthenticationService.Repository
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> GetByIdAsync(Guid id);
        Task<RefreshToken> GetByTokenAsync(string token);
        Task<IEnumerable<RefreshToken>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<RefreshToken>> GetActiveByUserIdAsync(Guid userId);
        Task AddAsync(RefreshToken refreshToken);
        Task UpdateAsync(RefreshToken refreshToken);
        Task DeleteAsync(Guid id);
        Task DeleteAllByUserIdAsync(Guid userId);
    }
} 