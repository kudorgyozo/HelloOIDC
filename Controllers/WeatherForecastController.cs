using HelloOIDC.Auth;
using IdentityModel.OidcClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HelloOIDC.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger) {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get() {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [Authorize]
        [HttpGet("testauth")]
        public string? GetName() {
            return User?.Identity?.Name ?? "(null)";
        }

        [HttpGet("token")]
        public IActionResult GetToken() {
            //your logic for login process
            //If login usrename and password are correct then proceed to generate token

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("keyasdfasdfasdfasdf"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var Sectoken = new JwtSecurityToken(
                null, null,
                new Claim[] {
                    new Claim(ClaimTypes.Role, "affdmin"),
                    new Claim(ClaimTypes.Name, "gyozo"),
                },
                expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(Sectoken);

            return Ok(token);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("testuser")]
        public UserResponse? GetName2() {
            return new UserResponse {
                Name = User?.Identity?.Name ?? "(null)",
                Role = User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "(null)"
            };
        }


        [RoleAuthorize("gyozo")]
        [HttpGet("testadmin")]
        public string? GetName3() {
            return User?.Identity?.Name ?? "(null)";
        }
    }
}

public class UserResponse {
    public string Name { get; set; }
    public string Role { get; set; }
}