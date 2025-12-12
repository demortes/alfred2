using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using AlfredBackend.Data;
using AlfredBackend.Services;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        // Configure PostgreSQL with Aspire
        builder.AddNpgsqlDbContext<AlfredDbContext>("alfreddb");

        // Configure MongoDB with Aspire
        builder.AddMongoDBClient("alfredaudit");

        // Register services
        builder.Services.AddScoped<IBotSettingsService, BotSettingsService>();
        builder.Services.AddScoped<IBotAccountService, BotAccountService>();
        builder.Services.AddSingleton<IAuditLogService, MongoAuditLogService>();

        // Add services to the container.
        builder.Services.AddAuthentication("Cookies")
            .AddCookie("Cookies", options =>
            {
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.Name = "Alfred.Auth";
                options.LoginPath = "/api/auth/login";
                options.LogoutPath = "/api/auth/logout";
            })
            .AddTwitch(options =>
            {
                options.SignInScheme = "Cookies";
                options.ClientId = builder.Configuration["Authentication:Twitch:ClientId"] ?? 
                    throw new InvalidOperationException("Twitch ClientId is not configured");
                options.ClientSecret = builder.Configuration["Authentication:Twitch:ClientSecret"] ?? 
                    throw new InvalidOperationException("Twitch ClientSecret is not configured");
                options.CallbackPath = "/api/auth/callback";
                options.SaveTokens = true;
                options.CorrelationCookie.SameSite = SameSiteMode.Lax;
                options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? 
                        throw new InvalidOperationException("JWT Issuer is not configured"),
                    ValidAudience = builder.Configuration["Jwt:Audience"] ?? 
                        throw new InvalidOperationException("JWT Audience is not configured"),
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? 
                            throw new InvalidOperationException("JWT Key is not configured")))
                };
            });

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend",
                policy =>
                {
                    var origins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>() 
                        ?? new[] { "http://localhost:4200" };
                    policy.WithOrigins(origins)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
        });

        var app = builder.Build();

        app.MapDefaultEndpoints();

        // Run EF Core migrations automatically
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AlfredDbContext>();
            await dbContext.Database.MigrateAsync(); // Use this instead for production
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseCors("AllowFrontend");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        await app.RunAsync();
    }
}