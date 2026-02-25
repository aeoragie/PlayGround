using PlayGround.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Aspire ServiceDefaults
builder.AddServiceDefaults();

// Controllers + OpenAPI
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
