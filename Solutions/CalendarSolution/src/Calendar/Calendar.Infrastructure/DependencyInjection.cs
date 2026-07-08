using Calendar.Application.Common.Interfaces;
using Calendar.Infrastructure.Consumers;
using Calendar.Infrastructure.Jobs;
using Calendar.Infrastructure.Persistence;
using Calendar.Infrastructure.Services;
using Hangfire;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Calendar.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<CalendarDbContext>(opts =>
            opts.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        services.AddScoped<ICalendarDbContext>(sp => sp.GetRequiredService<CalendarDbContext>());

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IReminderNotificationService, ReminderNotificationService>();

        services.AddHttpClient("notifications", c =>
            c.BaseAddress = new Uri(config["NotificationsService:BaseUrl"] ?? "http://localhost:7008"));

        services.AddMassTransit(x =>
        {
            x.AddConsumer<IntegrationVideosSyncedConsumer>();
            x.AddConsumer<CalendarBackgroundApprovedConsumer>();
            x.AddConsumer<CalendarBackgroundRejectedConsumer>();
            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(config["MessageBus:Host"]);
                cfg.ConfigureEndpoints(ctx);
            });
        });

        services.AddHangfire(cfg => cfg
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(config.GetConnectionString("DefaultConnection")));

        services.AddHangfireServer();

        services.AddScoped<ReminderDispatchJob>();

        return services;
    }
}
