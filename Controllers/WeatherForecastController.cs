using Azure.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using SimpleAPI.Dto;
using SimpleAPI.Dto.User;
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
        public IActionResult Login([FromBody] AccessRequest accessRequest)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, accessRequest.UserEmail),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var accessToken = _tokenService.GenerateAccessToken(claims);
            var successResponse = new BaseResponse<object>
            {
                Data = accessToken,
                Code = 200,
                Error = null
            };
            return Ok(successResponse);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("securedEndpoint")]
        public IActionResult SecuredEndpoint()
        {
            var userId = User.FindFirst(ClaimTypes.Name)?.Value;
            return Ok($"Hello, authenticated user with ID: {userId}");
        }
    }
}
