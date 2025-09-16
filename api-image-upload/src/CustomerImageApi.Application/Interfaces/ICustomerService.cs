using CustomerImageApi.DTOs.Requests;
using CustomerImageApi.DTOs.Responses;

namespace CustomerImageApi.Application.Interfaces;

public interface ICustomerService
{
    Task<CustomerResponse?> GetByIdAsync(int id);
    Task<IEnumerable<CustomerResponse>> GetAllAsync();
    Task<CustomerResponse> CreateAsync(CreateCustomerRequest request);
    Task<CustomerResponse?> UpdateAsync(int id, CreateCustomerRequest request);
    Task<bool> DeleteAsync(int id);
}