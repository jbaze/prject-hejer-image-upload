using CustomerImageApi.Domain.Entities;

namespace CustomerImageApi.Domain.Interfaces;

public interface ILeadImageRepository
{
    Task<LeadImage?> GetByIdAsync(int id);
    Task<IEnumerable<LeadImage>> GetByLeadIdAsync(int leadId);
    Task<LeadImage> CreateAsync(LeadImage image);
    Task DeleteAsync(int id);
    Task<int> GetImageCountByLeadIdAsync(int leadId);
}