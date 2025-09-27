using Microsoft.AspNetCore.Mvc;

namespace WolfGameBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RequenaController : ControllerBase
    {

        private readonly ILogger<RequenaController> _logger;

        public RequenaController(ILogger<RequenaController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "TestRequena")]
        public string Test()
        {
            return "Requena";
        }
    }
}
