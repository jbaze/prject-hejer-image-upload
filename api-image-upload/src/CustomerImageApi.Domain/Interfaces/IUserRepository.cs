using CustomerImageApi.Domain.Entities;

namespace CustomerImageApi.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByRefreshTokenAsync(string refreshToken);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> EmailExistsAsync(string email);
}