namespace Onboarding.API.Proxy;

public static class ProxyEndpoints
{
    public static void MapProxyEndpoints(this WebApplication app)
    {
        app.MapPost("/api/auth/login", (HttpContext c, IHttpClientFactory f) => Forward(c, f, "Identity", "/api/auth/login"));
        app.MapPost("/api/profiles", (HttpContext c, IHttpClientFactory f) => Forward(c, f, "Identity", "/api/profiles"));
    }

    private static async Task Forward(HttpContext ctx, IHttpClientFactory factory, string clientName, string path)
    {
        var client = factory.CreateClient(clientName);
        var request = new HttpRequestMessage(new HttpMethod(ctx.Request.Method), path);

        if (ctx.Request.ContentLength is > 0)
        {
            request.Content = new StreamContent(ctx.Request.Body);
            if (ctx.Request.ContentType is not null)
                request.Content.Headers.TryAddWithoutValidation("Content-Type", ctx.Request.ContentType);
        }
        if (ctx.Request.Headers.TryGetValue("Authorization", out var auth))
            request.Headers.TryAddWithoutValidation("Authorization", auth.ToString());

        var response = await client.SendAsync(request, ctx.RequestAborted);
        ctx.Response.StatusCode = (int)response.StatusCode;
        if (response.Content.Headers.ContentType is not null)

            ctx.Response.ContentType = response.Content.Headers.ContentType.ToString();
        await response.Content.CopyToAsync(ctx.Response.Body, ctx.RequestAborted);
    }
}
