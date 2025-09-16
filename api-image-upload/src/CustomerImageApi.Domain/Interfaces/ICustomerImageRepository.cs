using CustomerImageApi.Domain.Entities;

namespace CustomerImageApi.Domain.Interfaces;

public interface ICustomerImageRepository
{
    Task<CustomerImage?> GetByIdAsync(int id);
    Task<IEnumerable<CustomerImage>> GetByCustomerIdAsync(int customerId);
    Task<CustomerImage> CreateAsync(CustomerImage image);
    Task DeleteAsync(int id);
    Task<int> GetImageCountByCustomerIdAsync(int customerId);
}