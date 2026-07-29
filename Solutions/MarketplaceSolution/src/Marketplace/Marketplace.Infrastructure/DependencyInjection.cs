using Marketplace.Application.Common.Interfaces;
using Marketplace.Infrastructure.Persistence;
using Marketplace.Infrastructure.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Marketplace.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<MarketplaceDbContext>(opts =>
            opts.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        services.AddScoped<IMarketplaceDbContext>(sp => sp.GetRequiredService<MarketplaceDbContext>());

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(config["MessageBus:Host"]);
                cfg.ConfigureEndpoints(ctx);
            });
        });

        return services;
    }
}
