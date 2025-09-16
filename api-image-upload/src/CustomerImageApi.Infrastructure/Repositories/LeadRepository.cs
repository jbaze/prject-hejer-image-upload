using CustomerImageApi.Domain.Entities;
using CustomerImageApi.Domain.Interfaces;
using CustomerImageApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CustomerImageApi.Infrastructure.Repositories;

public class LeadRepository : ILeadRepository
{
    private readonly ApplicationDbContext _context;

    public LeadRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Lead?> GetByIdAsync(int id)
    {
        return await _context.Leads.FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<Lead?> GetByIdWithImagesAsync(int id)
    {
        return await _context.Leads
            .Include(l => l.Images)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<IEnumerable<Lead>> GetAllAsync()
    {
        return await _context.Leads
            .Include(l => l.Images)
            .ToListAsync();
    }

    public async Task<Lead> CreateAsync(Lead lead)
    {
        _context.Leads.Add(lead);
        await _context.SaveChangesAsync();
        return lead;
    }

    public async Task<Lead> UpdateAsync(Lead lead)
    {
        lead.UpdatedAt = DateTime.UtcNow;
        _context.Leads.Update(lead);
        await _context.SaveChangesAsync();
        return lead;
    }

    public async Task DeleteAsync(int id)
    {
        var lead = await _context.Leads.FindAsync(id);
        if (lead != null)
        {
            _context.Leads.Remove(lead);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Leads.AnyAsync(l => l.Id == id);
    }
}