using CustomerImageApi.Domain.Entities;
using CustomerImageApi.Domain.Interfaces;
using CustomerImageApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CustomerImageApi.Infrastructure.Repositories;

public class CustomerImageRepository : ICustomerImageRepository
{
    private readonly ApplicationDbContext _context;

    public CustomerImageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CustomerImage?> GetByIdAsync(int id)
    {
        return await _context.CustomerImages.FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<IEnumerable<CustomerImage>> GetByCustomerIdAsync(int customerId)
    {
        return await _context.CustomerImages
            .Where(i => i.CustomerId == customerId)
            .OrderBy(i => i.UploadedAt)
            .ToListAsync();
    }

    public async Task<CustomerImage> CreateAsync(CustomerImage image)
    {
        _context.CustomerImages.Add(image);
        await _context.SaveChangesAsync();
        return image;
    }

    public async Task DeleteAsync(int id)
    {
        var image = await _context.CustomerImages.FindAsync(id);
        if (image != null)
        {
            _context.CustomerImages.Remove(image);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> GetImageCountByCustomerIdAsync(int customerId)
    {
        return await _context.CustomerImages
            .CountAsync(i => i.CustomerId == customerId);
    }
}