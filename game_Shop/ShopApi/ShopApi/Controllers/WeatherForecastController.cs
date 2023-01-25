using DAL.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ShopApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly UserManager<UserEntity> _userManager;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,
            UserManager<UserEntity> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            var countUsers = _userManager.Users.Count();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = $"Count users {countUsers} -"+Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}