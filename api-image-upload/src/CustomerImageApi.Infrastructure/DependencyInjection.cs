using CustomerImageApi.Domain.Interfaces;
using CustomerImageApi.Infrastructure.Data;
using CustomerImageApi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerImageApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ILeadRepository, LeadRepository>();
        services.AddScoped<ICustomerImageRepository, CustomerImageRepository>();
        services.AddScoped<ILeadImageRepository, LeadImageRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}