using Identity.Application.Common.Interfaces;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Services;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<IdentityApplicationDbContext>(opts =>
            opts.UseSqlServer(config.GetConnectionString("DefaultConnection"))
                .UseOpenIddict());

        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(opts =>
        {
            opts.Password.RequiredLength = 8;
            opts.Password.RequireNonAlphanumeric = false;
            opts.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<IdentityApplicationDbContext>()
        .AddDefaultTokenProviders();

        services.AddOpenIddict()
            .AddCore(opts => opts.UseEntityFrameworkCore().UseDbContext<IdentityApplicationDbContext>())
            .AddValidation(opts =>
            {
                opts.UseLocalServer();
                opts.UseAspNetCore();
            });

        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(config["MessageBus:Host"]);
                cfg.ConfigureEndpoints(ctx);
            });
        });

        services.AddHttpClient("Google");

        services.AddScoped<IIdentityDbContext>(sp => sp.GetRequiredService<IdentityApplicationDbContext>());
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}
