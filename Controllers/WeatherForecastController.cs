using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleAPI.Services;
using System.Security.Claims;

namespace SimpleAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly TokenService _tokenService;

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, TokenService tokenService)
        {
            _logger = logger;
            _tokenService = tokenService;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost("login")]
        public IActionResult Login()
        {
            // Implement login logic and generate tokens
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "username"),
            new Claim(ClaimTypes.Role, "YourUserRole")
                // Add more claims as needed
        };

            var accessToken = _tokenService.GenerateAccessToken(claims);
            // Implement refresh token logic if needed

            return Ok(new { AccessToken = accessToken, RefreshToken = "your_refresh_token" });
        }

        [Authorize]
        [HttpGet("securedEndpoint")]
        public IActionResult SecuredEndpoint()
        {
            // This endpoint is secured, and only authenticated users with a valid token can access it.
            // The user information is available through the User property of the ControllerBase.
            var userId = User.FindFirst(ClaimTypes.Name)?.Value;
            return Ok($"Hello, authenticated user with ID: {userId}");
        }
    }
}
