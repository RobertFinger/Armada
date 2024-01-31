using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Authentication.Controllers;

// https://www.youtube.com/watch?v=UwruwHl3BlU
// https://www.youtube.com/watch?v=TDY_DtTEkes

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : Controller
{
    private readonly IConfiguration? _configuration;
    private readonly ILogger<AuthenticationController> _logger;
    private static readonly User _user = new();

    public AuthenticationController(IConfiguration? config, ILogger<AuthenticationController> logger)
    {
        _configuration = config;
        _logger = logger;   
    }

    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(UserResponse request)
    {
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        _user.Id = Guid.NewGuid();
        _user.Username = request.Username;
        _user.PasswordHash = passwordHash;

        await SendUserInfoToGateway();

        return Ok(_user);
    }

    private async Task SendUserInfoToGateway()
    {
        string apiUrl = "https://localhost:6001/SaveUser";

        using (HttpClient client = new HttpClient())
        {
            try
            {
                var requestJson = JsonSerializer.Serialize(_user);
                var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    _logger.LogError($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }
        }

    }


    [HttpPost("login")]
    public ActionResult<User> Login(UserResponse request)
    {
        // search database for user.

        if (_user.Username != request.Username)
        {
            return BadRequest("Incorrect Credentials");
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, _user.PasswordHash))
        {
            return BadRequest("Incorrect Credentials");
        }

        var token = CreateToken(_user);
        return Ok(token);
    }

    [HttpPost("VerifyToken"), Authorize]
    public ActionResult<User> VerifyToken(string userId)
    {

        if (User.FindFirst(ClaimTypes.Name)?.Value != userId)
        {
            return BadRequest("Incorrect Credentials");
        }

        return Ok(true);
    }



    private string CreateToken(User user)
    {

        List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, "User")
            };

        var tokenKey = _configuration[AuthConstants.ApiKeySectionName];

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds
            );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }
}
