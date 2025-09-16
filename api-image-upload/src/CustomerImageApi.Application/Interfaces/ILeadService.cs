using CustomerImageApi.DTOs.Requests;
using CustomerImageApi.DTOs.Responses;

namespace CustomerImageApi.Application.Interfaces;

public interface ILeadService
{
    Task<LeadResponse?> GetByIdAsync(int id);
    Task<IEnumerable<LeadResponse>> GetAllAsync();
    Task<LeadResponse> CreateAsync(CreateLeadRequest request);
    Task<LeadResponse?> UpdateAsync(int id, CreateLeadRequest request);
    Task<bool> DeleteAsync(int id);
}