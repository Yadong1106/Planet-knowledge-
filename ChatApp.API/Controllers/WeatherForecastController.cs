using Microsoft.AspNetCore.Mvc;
using ChatApp.API.Untity;
using ChatApp.API.Services;
namespace ChatApp.API.Controllers
{
    [ApiController]
    [Route("weather")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private LoginService _loginService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, LoginService loginService)
        {
            _logger = logger;
            _loginService = loginService;
        }

        [HttpGet("GetWeatherForecast", Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost]
        [Route("login")]
        public string Login([FromBody] User user)
        {
            var result = _loginService.ValidateLoginInfo(user);
            if (result)
            {
                return "success";
            }
            return "false";
        }

        [HttpPost]
        [Route("register")]
        public string Register([FromBody] User user) 
        {
            if (user == null)
            {
                throw new ArgumentNullException("request body can not be empty");
            }

            _loginService.RegisterUser(user);

            return "OK";
        }

        [HttpPut]
        [Route("validateCode/{email}")]
        public string SendValidateCode(string email)
        {
            if (email == null)
            {
                throw new ArgumentNullException("Email address can not be empty");
            }

            _loginService.SendValidateCode(email);

            return "send OK";
        }

        [HttpDelete]
        [Route("delete/{name}")]
        public string DeleteUser(string name)
        {
            //string name = HttpContext.Request.RouteValues["name"] as string;
            _loginService.DeleteUser(name);
            return "delete OK";
        }

        [HttpPatch]
        [Route("update/{name}")]
        public string UpdateUser(string name, [FromBody] string newPassword)
        {

            _loginService.UpdateUser(name, newPassword);
            return "Update success";
        }
    }
}