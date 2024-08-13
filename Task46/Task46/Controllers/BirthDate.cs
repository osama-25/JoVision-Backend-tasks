using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Task46.Models;

namespace Task46.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BirthDate : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromForm] UserInfo Info)
        {
            if (Info.name == null) Info.name = "anonymous";

            if (Info.year == null || Info.month == null || Info.day == null)
                return Ok($"Hello {Info.name},  I can’t calculate your age without knowing your birthdate!");
            else
            {
                int age = (int)(DateTime.Today.Year - Info.year);
                int nowmonth = DateTime.Today.Month;
                int nowday = DateTime.Today.Day;
                if (Info.month > nowmonth || (Info.month == nowmonth && Info.day > nowday))
                {
                    age--;
                }

                return Ok($"Hello {Info.name}, your age is {age}");
            }

        }
    }
}
