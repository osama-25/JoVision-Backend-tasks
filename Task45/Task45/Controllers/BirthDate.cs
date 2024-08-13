using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Task45.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BirthDate : ControllerBase
    {
        [HttpGet]
        public IActionResult Get([FromQuery] int year, [FromQuery] int month, [FromQuery] int day, [FromQuery] string name = "")
        {
            if (name == "") name = "anonymous";

            if (year == 0 || month == 0 || day == 0)
                return Ok($"Hello {name},  I can’t calculate your age without knowing your birthdate!");
            else
            {
                int age = DateTime.Today.Year - year;
                int nowmonth = DateTime.Today.Month;
                int nowday = DateTime.Today.Day;
                if(month > nowmonth || (month == nowmonth && day > nowday))
                {
                    age--;
                }

                return Ok($"Hello {name}, your age is {age}");
            }

        }
    }
}
