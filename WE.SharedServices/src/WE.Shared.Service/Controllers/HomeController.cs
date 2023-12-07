using Common.ControllerBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WE.Shared.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Home()
        {
            return Ok();
        }
    }
}
