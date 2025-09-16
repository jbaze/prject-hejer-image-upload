using CustomerImageApi.Domain.Entities;

namespace CustomerImageApi.Domain.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(int id);
    Task<Customer?> GetByIdWithImagesAsync(int id);
    Task<IEnumerable<Customer>> GetAllAsync();
    Task<Customer> CreateAsync(Customer customer);
    Task<Customer> UpdateAsync(Customer customer);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}