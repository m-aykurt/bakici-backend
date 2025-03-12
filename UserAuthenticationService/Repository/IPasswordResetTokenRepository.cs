using System;
using System.Threading.Tasks;
using UserAuthenticationService.Domain;

namespace UserAuthenticationService.Repository
{
    public interface IPasswordResetTokenRepository
    {
        Task<PasswordResetToken> GetByIdAsync(Guid id);
        Task<PasswordResetToken> GetByTokenAsync(string token);
        Task<PasswordResetToken> GetValidTokenByUserIdAsync(Guid userId);
        Task AddAsync(PasswordResetToken passwordResetToken);
        Task UpdateAsync(PasswordResetToken passwordResetToken);
        Task DeleteAsync(Guid id);
        Task DeleteAllByUserIdAsync(Guid userId);
        Task DeleteExpiredTokensAsync();
    }
} 