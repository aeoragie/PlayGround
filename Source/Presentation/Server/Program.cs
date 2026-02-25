using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PlayGround.Application.Interfaces;
using PlayGround.Infrastructure.Database;
using PlayGround.Persistence.Repositories;
using PlayGround.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Aspire ServiceDefaults
builder.AddServiceDefaults();

// Controllers + OpenAPI
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Database Configuration (Dapper)
builder.Services.Configure<DatabaseConfiguration>(
    builder.Configuration.GetSection(DatabaseConfiguration.Section));

// DI: Auth
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured")))
    };
});

builder.Services.AddAuthorization();

// SignalR
builder.Services.AddSignalR();

// CORS (Blazor WASM 클라이언트 허용)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [])
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Aspire 기본 엔드포인트 (/health, /alive)
app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// Blazor WASM 정적 파일 서빙
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.MapControllers();

// SignalR 허브
// app.MapHub<NotificationHub>("/hubs/notification");

// Blazor WASM 폴백
app.MapFallbackToFile("index.html");

app.Run();
