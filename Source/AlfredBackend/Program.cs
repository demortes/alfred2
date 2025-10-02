using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "Twitch";
            })
            .AddTwitch(options =>
            {
                options.ClientId = builder.Configuration["Authentication:Twitch:ClientId"] ?? 
                    throw new InvalidOperationException("Twitch ClientId is not configured");
                options.ClientSecret = builder.Configuration["Authentication:Twitch:ClientSecret"] ?? 
                    throw new InvalidOperationException("Twitch ClientSecret is not configured");
                options.CallbackPath = "/api/auth/callback";
                options.SaveTokens = true;
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