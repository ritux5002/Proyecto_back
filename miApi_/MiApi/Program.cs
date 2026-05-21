using MiApp.Application.UseCases.Auth;
using MiApp.Domain.Interfaces;
using MiApp.Infrastructure;
using MiApp.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add CORS Configuration - Reemplaza con tu dominio frontend real
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")  // Reemplaza con tu URL real
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Add services to the container
builder.Services.AddControllers();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Infrastructure Services
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add Application Services - Token Service
builder.Services.AddScoped<ITokenService>(sp => new TokenService(
    config["Jwt:Key"]!,
    config["Jwt:Issuer"]!,
    config["Jwt:Audience"]!,
    int.Parse(config["Jwt:ExpirationHours"]!)));

// Login UseCase
builder.Services.AddScoped(sp => new LoginUseCase(
    sp.GetRequiredService<IUserRepository>(),
    sp.GetRequiredService<ITokenService>()));

// Add JWT Authentication with Enhanced Security
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = config["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = config["Jwt:Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.Zero, // Sin tolerancia de tiempo
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(config["Jwt:Key"]!))
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Enable CORS
app.UseCors("AllowFrontend");

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MiApp API v1");
        c.RoutePrefix = string.Empty;
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    app.UseHsts(); // Agregar HSTS en producción
}

app.UseAuthentication();  // Primero: identifica quién es
app.UseAuthorization();   // Segundo: decide qué puede hacer

app.MapControllers();

app.Run();