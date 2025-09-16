using CustomerImageApi.Domain.Entities;
using CustomerImageApi.Domain.Interfaces;
using CustomerImageApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CustomerImageApi.Infrastructure.Repositories;

public class LeadImageRepository : ILeadImageRepository
{
    private readonly ApplicationDbContext _context;

    public LeadImageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LeadImage?> GetByIdAsync(int id)
    {
        return await _context.LeadImages.FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<IEnumerable<LeadImage>> GetByLeadIdAsync(int leadId)
    {
        return await _context.LeadImages
            .Where(i => i.LeadId == leadId)
            .OrderBy(i => i.UploadedAt)
            .ToListAsync();
    }

    public async Task<LeadImage> CreateAsync(LeadImage image)
    {
        _context.LeadImages.Add(image);
        await _context.SaveChangesAsync();
        return image;
    }

    public async Task DeleteAsync(int id)
    {
        var image = await _context.LeadImages.FindAsync(id);
        if (image != null)
        {
            _context.LeadImages.Remove(image);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> GetImageCountByLeadIdAsync(int leadId)
    {
        return await _context.LeadImages
            .CountAsync(i => i.LeadId == leadId);
    }
}