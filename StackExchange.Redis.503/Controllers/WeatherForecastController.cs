using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace StackExchange.Redis._503.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IRedisCacheClient _redis;

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };


        public WeatherForecastController(
            IRedisCacheClient redis
            )
        {
            _redis = redis;

            Init();
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var keys = _redis.Db1.SearchKeys("weatherForecast:*");
            var data = _redis.Db1.GetAll<WeatherForecast>(keys);

            var result = data.Values.OrderByDescending(o => o.Date).ToList();
            return result;
        }

        private void Init()
        {
            var rng = new Random();
            var data = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            });

            foreach (var item in data)
            {
                _redis.Db1.Add($"weatherForecast:{item.Date}", item);
            }
        }
    }
}
