using ApiHost.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiHost.Controllers
{
    public class WeatherForecastController : ControllerBase
    {  private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
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

        [HttpGet("all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await DbHelper.QueryAsync<User>("select product_id as userid,name as UserName from products");
            return Ok(users);
        }
         
        
        [HttpGet("api/pgtest")]
        public async Task<IActionResult> pgtest()
        {
            var users = await DbHelper.QueryAsync<pgtest>("SELECT * FROM test_users");
            return Ok(users);
        }


    }
}
