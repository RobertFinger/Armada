using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace Authentication.Controllers
{
    // https://www.youtube.com/watch?v=UwruwHl3BlU
    // https://www.youtube.com/watch?v=TDY_DtTEkes

    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly IConfiguration? _configuration;
        private static readonly User _user = new();

        public AuthenticationController(IConfiguration? config)
        {
            _configuration = config;
        }

        [HttpPost("register")]
        public ActionResult<User> Register(UserResponse request)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            _user.Id = Guid.NewGuid();
            _user.Username = request.Username;
            _user.PasswordHash = passwordHash;

            // save to database

            return Ok(_user);
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

            // get key from database, not app settings!  This is just a placeholder
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
}
