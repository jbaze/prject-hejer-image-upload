using CustomerImageApi.Application.Interfaces;
using CustomerImageApi.Application.Services;
using CustomerImageApi.Application.Mapping;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerImageApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ILeadService, LeadService>();
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        services.AddAutoMapper(typeof(DependencyInjection).Assembly);

        return services;
    }
}