using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using YouTube.Application.Common.Interfaces;
using YouTube.Infrastructure.Consumers;
using YouTube.Infrastructure.Persistence;
using YouTube.Infrastructure.Services;

namespace YouTube.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<YouTubeDbContext>(opts =>
            opts.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        services.AddScoped<IYouTubeDbContext>(sp => sp.GetRequiredService<YouTubeDbContext>());

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddRefitClient<IYouTubeApiClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(
                config["ExternalApis:YouTube:BaseUrl"] ?? "https://www.googleapis.com"));

        services.AddMassTransit(x =>
        {
            x.AddConsumer<YouTubeSyncRequestedConsumer>();
            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(config["MessageBus:Host"]);
                cfg.ConfigureEndpoints(ctx);
            });
        });

        return services;
    }
}
