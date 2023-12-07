using Common.ControllerBase;
using Common.Mongodb.Collections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WE.SharedService.Application.Dtos;
using WE.SharedService.Application.Models.ApplicationName;
using WE.SharedService.Application.Services;
using WE.Shared.Service.Domain.Entities;

namespace WE.Shared.Service.Controllers
{

    [ApiController]
    [Route("[controller]")]
    [AllowAnonymous]
    public class ApplicationNameController : ApiControllerBase
    {
        private readonly IApplicationNameService _applicationNameService;

        public ApplicationNameController(IApplicationNameService applicationNameService)
        {
            this._applicationNameService = applicationNameService ?? throw new ArgumentNullException(nameof(applicationNameService));
        }

        [HttpGet]
        [Produces(typeof(PagedList<ApplicationName>))]
        public async Task<IActionResult> GetAll([FromQuery]SearchApplicationNameViewModel searchModel)
        {
            var result = await this._applicationNameService.GetAllApplicationName(searchModel);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {

            var result = await _applicationNameService.CreateApplicationName(name);

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id,string name)
        {

            var result = await _applicationNameService.UpdateApplicationName(id,name);
            return Ok(result);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _applicationNameService.DeleteApplicationName(id);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Produces(typeof(ApplicationName))]
        public async Task<IActionResult> GetApplicationName(string id)
        {
            var result = _applicationNameService.GetApplicationName(id);
            return Ok(result);
        }
    }
}
