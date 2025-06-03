using LMTempliq.Backend.API.Managers;
using LMTempliq.Backend.API.Middlewares;
using LMTempliq.Backend.API.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.Configure<List<SenderIdentity>>(builder.Configuration.GetSection("SenderIdentities"));

builder.Services.AddSingleton<SenderIdentityManager>();
builder.Services.AddSingleton<TemplateManager>();
builder.Services.AddSingleton<MailManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();
app.UseMiddleware<ApiKeyMiddleware>();

app.MapControllers();

app.Run();