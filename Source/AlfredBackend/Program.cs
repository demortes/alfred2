using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

public class Program
{
    /// <summary>
    /// Builds, configures, and runs the ASP.NET Core web application using the provided command-line arguments.
    /// </summary>
    /// <param name="args">Command-line arguments passed to the application.</param>
    /// <remarks>
    /// Configures authentication (cookie scheme, Twitch external login, and JWT bearer validation), adds controllers and Swagger/OpenAPI services,
    /// registers a CORS policy named "AllowSpecificOrigin" using origins from configuration, and sets up the HTTP request pipeline
    /// (Swagger in development, CORS, HTTPS redirection, authentication, authorization, and controller endpoints) before running the app.
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown when required configuration values for Twitch or JWT (ClientId, ClientSecret, Issuer, Audience, or Key) are missing.</exception>
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthentication("Cookies")
            .AddCookie("Cookies", options =>
            {
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.Name = "Alfred.Auth";
                options.LoginPath = "/api/auth/login";
                options.LogoutPath = "/api/auth/logout";
                options.Events.OnSigningIn = context =>
                {
                    var tokens = context.Properties.GetTokens();
                    var auth_token = tokens.FirstOrDefault(t => t.Name == "access_token");
                    if (auth_token != null)
                    {
                        context.Properties.StoreTokens(tokens);
                    }
                    return Task.CompletedTask;
                };
            })
            .AddTwitch(options =>
            {
                options.SignInScheme = "Cookies";
                options.ClientId = builder.Configuration["Authentication:Twitch:ClientId"] ?? 
                    throw new InvalidOperationException("Twitch ClientId is not configured");
                options.ClientSecret = builder.Configuration["Authentication:Twitch:ClientSecret"] ?? 
                    throw new InvalidOperationException("Twitch ClientSecret is not configured");
                options.CallbackPath = "/signin-twitch";
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
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin",
                builder2 =>
                {
                    var origins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>();
                    if (origins != null)
                    {
                        builder2.WithOrigins(origins)
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    }
                });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors("AllowSpecificOrigin");

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}