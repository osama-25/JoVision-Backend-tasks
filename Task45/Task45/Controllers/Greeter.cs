using Microsoft.AspNetCore.Mvc;

namespace Task44.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Greeter : ControllerBase
    {
        [HttpGet]
        public IActionResult Get([FromQuery] string name = "")
        {
            if (name == "") name = "anonymous";
            return Ok($"Hello {name}");
        }
    }
}
