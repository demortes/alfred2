using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AlfredBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of <see cref="AuthController"/> with the provided configuration source.
        /// </summary>
        /// <param name="configuration">Application configuration provider used to read JWT settings and CORS origins.</param>
        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Initiates an external authentication challenge using the Twitch scheme.
        /// On success, the user is redirected to the frontend URL.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> that issues a Challenge for the "Twitch" authentication scheme, causing the client to be redirected to the external provider.</returns>
        [HttpGet("login")]
        public IActionResult Login()
        {
            var frontendUrl = _configuration.GetSection("Cors:Origins").Get<string[]>()?[0] ?? 
                throw new InvalidOperationException("Frontend URL is not configured");

            var properties = new AuthenticationProperties 
            { 
                RedirectUri = frontendUrl,
                IsPersistent = true,
            };
            return Challenge(properties, "Twitch");
        }

        /// <summary>
        /// After successful external authentication, this endpoint generates and returns a JWT for the authenticated user.
        /// </summary>
        /// <returns>A JSON object containing the JWT, or a <see cref="BadRequestResult"/> if the Twitch access token is missing.</returns>
        /// <exception cref="InvalidOperationException">Thrown when required JWT configuration values are missing.</exception>
        [Authorize]
        [HttpGet("token")]
        public async Task<IActionResult> Token()
        {
            var twitchToken = await HttpContext.GetTokenAsync("access_token");
            if (string.IsNullOrEmpty(twitchToken))
            {
                return BadRequest("Failed to obtain Twitch token");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? 
                throw new InvalidOperationException("JWT Key is not configured"));
            
            var claims = User.Claims.ToList();
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

            return Ok(new { token = jwt });
        }
    }
}