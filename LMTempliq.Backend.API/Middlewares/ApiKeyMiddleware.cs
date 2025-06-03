namespace LMTempliq.Backend.API.Middlewares;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _config;

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration config)
    {
        _next = next;
        _config = config;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("X-Api-Key", out var apiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("api key missing");
            return;
        }

        var validKey = _config["Auth:ApiKey"];
        if (apiKey != validKey)
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("api key invalid");
            return;
        }

        await _next(context);
    }
}
