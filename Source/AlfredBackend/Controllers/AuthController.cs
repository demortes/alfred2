using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AlfredBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            var frontendUrl = _configuration.GetSection("Cors:Origins").Get<string[]>()?[0] ?? 
                throw new InvalidOperationException("Frontend URL is not configured");

            var properties = new AuthenticationProperties 
            { 
                RedirectUri = "/api/auth/callback",
                IsPersistent = true,
                Items =
                {
                    { "returnUrl", frontendUrl }
                }
            };
            return Challenge(properties, "Twitch");
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync("Twitch");

            if (!authenticateResult.Succeeded)
            {
                return BadRequest("Authentication failed");
            }

            var twitchToken = await HttpContext.GetTokenAsync("Twitch", "access_token");
            if (string.IsNullOrEmpty(twitchToken))
            {
                return BadRequest("Failed to obtain Twitch token");
            }

            // Generate our own JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? 
                throw new InvalidOperationException("JWT Key is not configured"));
            
            var claims = authenticateResult.Principal?.Claims.ToList() ?? new List<Claim>();
            // Add the Twitch token as a claim if you need it later
            claims.Add(new Claim("twitch_token", twitchToken));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), 
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"] ?? 
                    throw new InvalidOperationException("JWT Issuer is not configured"),
                Audience = _configuration["Jwt:Audience"] ?? 
                    throw new InvalidOperationException("JWT Audience is not configured")
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            // Redirect to frontend with token
            var frontendUrl = _configuration.GetSection("Cors:Origins").Get<string[]>()?[0] ?? 
                throw new InvalidOperationException("Frontend URL is not configured");
            return Redirect($"{frontendUrl}?token={jwt}");
        }
    }
}