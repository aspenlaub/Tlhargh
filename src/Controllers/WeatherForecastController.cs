using Microsoft.AspNetCore.Mvc;

namespace Aspenlaub.Net.GitHub.CSharp.Tlhargh.Controllers;
[ApiController, Route("[controller]")]
public class WeatherForecastController(ILogger<WeatherForecastController> logger) : ControllerBase {
    private static readonly string[] _summaries = [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    private readonly ILogger<WeatherForecastController> _Logger = logger;

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get() {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast {
                             Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                             TemperatureC = Random.Shared.Next(-20, 55),
                             Summary = _summaries[Random.Shared.Next(_summaries.Length)]
                         })
                         .ToArray();
    }
}
