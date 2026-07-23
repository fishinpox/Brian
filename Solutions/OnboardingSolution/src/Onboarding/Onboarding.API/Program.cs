using Onboarding.API.Proxy;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddHttpClient("Identity", c =>
    c.BaseAddress = new Uri(builder.Configuration["Services:IdentityBaseUrl"]!));

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseSerilogRequestLogging();
if (app.Environment.IsDevelopment()) { app.MapOpenApi(); app.MapScalarApiReference(); }
app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.CacheControl = "no-cache, no-store, must-revalidate";
        ctx.Context.Response.Headers.Pragma = "no-cache";
        ctx.Context.Response.Headers.Expires = "0";
    }
});
app.MapProxyEndpoints();
app.MapGet("/health", () => Results.Ok(new { service = "onboarding", status = "healthy" })).AllowAnonymous();
app.Run();

public partial class Program { }
