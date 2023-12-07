using Common.ControllerBase;
using Microsoft.AspNetCore.Mvc;

namespace WE.Shared.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthCheckController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> HealthCheck()
        {
            return Ok();
        }
    }
}
