using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AlfredBackend.Models;
using AlfredBackend.Services;

namespace AlfredBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BotAccountController : ControllerBase
    {
        private readonly IBotAccountService _botAccountService;
        private readonly ILogger<BotAccountController> _logger;

        public BotAccountController(IBotAccountService botAccountService, ILogger<BotAccountController> logger)
        {
            _botAccountService = botAccountService;
            _logger = logger;
        }

        private string GetTwitchUserId()
        {
            return User.FindFirst("sub")?.Value 
                ?? User.Identity?.Name 
                ?? throw new UnauthorizedAccessException("User ID not found");
        }

        [HttpGet]
        public async Task<IActionResult> GetBotAccounts()
        {
            try
            {
                var userId = GetTwitchUserId();
                var accounts = await _botAccountService.GetUserBotAccountsAsync(userId);
                
                // Don't expose sensitive tokens in the response
                var safeAccounts = accounts.Select(a => new
                {
                    a.Id,
                    a.BotTwitchId,
                    a.BotUsername,
                    a.BotDisplayName,
                    a.IsActive,
                    a.CreatedAt,
                    TokenExpiresAt = a.TokenExpiresAt,
                    IsTokenExpired = a.TokenExpiresAt < DateTime.UtcNow
                });
                
                return Ok(safeAccounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bot accounts");
                return StatusCode(500, "An error occurred while retrieving bot accounts");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBotAccount(int id)
        {
            try
            {
                var botAccount = await _botAccountService.GetBotAccountAsync(id);
                
                if (botAccount == null)
                {
                    return NotFound($"Bot account {id} not found");
                }

                var userId = GetTwitchUserId();
                if (botAccount.OwnerTwitchUserId != userId)
                {
                    return Forbid();
                }

                // Don't expose tokens
                return Ok(new
                {
                    botAccount.Id,
                    botAccount.BotTwitchId,
                    botAccount.BotUsername,
                    botAccount.BotDisplayName,
                    botAccount.IsActive,
                    botAccount.CreatedAt,
                    TokenExpiresAt = botAccount.TokenExpiresAt,
                    IsTokenExpired = botAccount.TokenExpiresAt < DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving bot account {id}");
                return StatusCode(500, "An error occurred while retrieving the bot account");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateBotAccount([FromBody] CreateBotAccountRequest request)
        {
            try
            {
                var userId = GetTwitchUserId();

                var botAccount = new BotAccount
{
                    OwnerTwitchUserId = userId,
                    BotTwitchId = request.BotTwitchId,
                    BotUsername = request.BotUsername,
                    BotDisplayName = request.BotDisplayName,
                    AccessToken = request.AccessToken,
                    RefreshToken = request.RefreshToken,
                    TokenExpiresAt = request.TokenExpiresAt,
                    IrcToken = request.IrcToken,
                    IsActive = true
                };

                var created = await _botAccountService.CreateBotAccountAsync(botAccount);
                _logger.LogInformation($"Created bot account {created.Id} for user {userId}");

                return CreatedAtAction(nameof(GetBotAccount), new { id = created.Id }, new { id = created.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating bot account");
                return StatusCode(500, "An error occurred while creating the bot account");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBotAccount(int id)
        {
            try
            {
                var userId = GetTwitchUserId();
                var success = await _botAccountService.DeleteBotAccountAsync(id, userId);

                if (!success)
                {
                    return NotFound($"Bot account {id} not found or you don't have permission");
                }

                _logger.LogInformation($"Deleted bot account {id} for user {userId}");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting bot account {id}");
                return StatusCode(500, "An error occurred while deleting the bot account");
            }
        }

        [HttpPost("{id}/refresh-token")]
        public async Task<IActionResult> RefreshToken(int id, [FromBody] RefreshTokenRequest request)
        {
            try
            {
                // TODO: Add authorization check
                
                var success = await _botAccountService.RefreshTokenAsync(
                    id, 
                    request.AccessToken, 
                    request.RefreshToken, 
                    request.ExpiresAt);

                if (!success)
                {
                    return NotFound($"Bot account {id} not found");
                }

                _logger.LogInformation($"Refreshed token for bot account {id}");
                return Ok(new { message = "Token refreshed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error refreshing token for bot account {id}");
                return StatusCode(500, "An error occurred while refreshing the token");
            }
        }
    }

    public record CreateBotAccountRequest(
        string BotTwitchId,
        string BotUsername,
        string? BotDisplayName,
        string AccessToken,
        string RefreshToken,
        DateTime TokenExpiresAt,
        string? IrcToken
    );

    public record RefreshTokenRequest(
        string AccessToken,
        string RefreshToken,
        DateTime ExpiresAt
    );
}
