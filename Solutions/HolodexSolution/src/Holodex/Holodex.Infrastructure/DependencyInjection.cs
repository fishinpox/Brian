using Holodex.Application.Common.Interfaces;
using Holodex.Infrastructure.Consumers;
using Holodex.Infrastructure.Persistence;
using Holodex.Infrastructure.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Holodex.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<HolodexDbContext>(opts =>
            opts.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        services.AddScoped<IHolodexDbContext>(sp => sp.GetRequiredService<HolodexDbContext>());

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddRefitClient<IHolodexApiClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(
                config["ExternalApis:Holodex:BaseUrl"] ?? "https://holodex.net/api/v2"));

        services.AddMassTransit(x =>
        {
            x.AddConsumer<HolodexSyncRequestedConsumer>();
            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(config["MessageBus:Host"]);
                cfg.ConfigureEndpoints(ctx);
            });
        });

        return services;
    }
}
