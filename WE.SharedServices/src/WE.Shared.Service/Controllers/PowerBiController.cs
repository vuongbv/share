using Common.ControllerBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WE.SharedService.Application.Services;

namespace WE.Shared.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PowerBiController : ControllerBase
    {
        private readonly PowerBiService _powerBiService;

        public PowerBiController(PowerBiService powerBiService)
        {
            _powerBiService = powerBiService;
        }

        [HttpGet]
        [Produces(typeof(string))]
        public async Task<IActionResult> GetToken()
        {
            return Ok();
        }
        [HttpGet("report")]
        [Produces(typeof(EmbeddedReportViewModel))]
        public async Task<IActionResult> GetReport(Guid groupId,Guid reportId)
        {
            var result = await _powerBiService.GetReport(groupId, reportId);
            return Ok(result);
        }
    }
}