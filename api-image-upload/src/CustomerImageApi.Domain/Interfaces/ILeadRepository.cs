using CustomerImageApi.Domain.Entities;

namespace CustomerImageApi.Domain.Interfaces;

public interface ILeadRepository
{
    Task<Lead?> GetByIdAsync(int id);
    Task<Lead?> GetByIdWithImagesAsync(int id);
    Task<IEnumerable<Lead>> GetAllAsync();
    Task<Lead> CreateAsync(Lead lead);
    Task<Lead> UpdateAsync(Lead lead);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}