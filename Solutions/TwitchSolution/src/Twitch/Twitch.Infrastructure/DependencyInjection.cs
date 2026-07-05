using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Twitch.Application.Common.Interfaces;
using Twitch.Infrastructure.Consumers;
using Twitch.Infrastructure.Persistence;
using Twitch.Infrastructure.Services;

namespace Twitch.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<TwitchDbContext>(opts =>
            opts.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        services.AddScoped<ITwitchDbContext>(sp => sp.GetRequiredService<TwitchDbContext>());

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddRefitClient<ITwitchApiClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(
                config["ExternalApis:Twitch:BaseUrl"] ?? "https://api.twitch.tv"));

        services.AddMassTransit(x =>
        {
            x.AddConsumer<TwitchSyncRequestedConsumer>();
            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(config["MessageBus:Host"]);
                cfg.ConfigureEndpoints(ctx);
            });
        });

        return services;
    }
}
