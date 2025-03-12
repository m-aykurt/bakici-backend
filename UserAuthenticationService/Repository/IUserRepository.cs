using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserAuthenticationService.Domain;

namespace UserAuthenticationService.Repository
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(Guid id);
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByExternalLoginAsync(string provider, string providerKey);
        Task<IEnumerable<User>> GetAllAsync();
        Task<IEnumerable<User>> GetByUserTypeAsync(UserType userType);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsByEmailAsync(string email);
    }
} 