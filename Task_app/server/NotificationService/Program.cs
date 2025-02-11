using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotificationService.Hubs;
using NotificationService.Messaging;
using RabbitMQ.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:5002");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policyBuilder =>
    {
        policyBuilder
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://securetoken.google.com/taskapp-e90d2";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://securetoken.google.com/taskapp-e90d2",
            ValidateAudience = true,
            ValidAudience = "taskapp-e90d2",
            ValidateLifetime = true,
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();

builder.Services.AddSignalR();
var rabbitMqUri = builder.Configuration["RabbitMQ:ConnectionString"];
builder.Services.AddSingleton<IConnectionFactory>(_ => new ConnectionFactory
{
    Uri = new Uri(rabbitMqUri),
    DispatchConsumersAsync = true
});
builder.Services.AddSingleton<NotificationRepository>();
builder.Services.AddHostedService<RabbitMqListener>();

var app = builder.Build();
app.UseRouting();

app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<NotificationHub>("/notifications");
});

app.Run();
