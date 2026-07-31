using Chat.Application.Common.Interfaces;
using Chat.Infrastructure.Persistence;
using Chat.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Chat.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<ChatDbContext>(opts =>
            opts.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        services.AddScoped<IChatDbContext>(sp => sp.GetRequiredService<ChatDbContext>());

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<ICredentialEncryptionService, AesCredentialEncryptionService>();

        services.AddRefitClient<IStoatApiClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(
                config["ExternalApis:Stoat:BaseUrl"] ?? "http://localhost:8880/api"));

        return services;
    }
}
