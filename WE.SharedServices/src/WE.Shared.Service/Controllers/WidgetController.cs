using Common.ControllerBase;
using Common.Mongodb.Collections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WE.SharedService.Application.Dtos;
using WE.SharedService.Application.Models.Widgets;
using WE.SharedService.Application.Services;

namespace WE.Shared.Service.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class WidgetController : ApiControllerBase
    {
        private readonly IWidgetService _widgetService;

        public WidgetController(IWidgetService widgetService)
        {
            this._widgetService = widgetService ?? throw new ArgumentNullException(nameof(widgetService));
        }

        [HttpGet]
        [Produces(typeof(PagedList<WidgetDto>))]
        public async Task<IActionResult> GetAll([FromQuery]SearchWidgetViewModel searchModel)
        {
            var result = await this._widgetService.GetAllWidget(searchModel);

            return Ok(result);
        }

        [HttpPost]
        [Produces(typeof(WidgetDto))]
        public async Task<IActionResult> Create([FromBody] CreateWidgetModel model)
        {
            var validator = new CreateWidgetModelValidator().Validate(model);

            if (!validator.IsValid)
            {
                return BadRequest(validator.Errors);
            }
            var result = await _widgetService.CreateWidget(model);

            return Ok(result);
        }

        [HttpPost("position")]
        public async Task<IActionResult> SavePosition([FromBody] List<UpdateWidgetPositionModel> models)
        {

            var result = await _widgetService.UpdateWidgetPosition(models);

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateWidgetModel model)
        {
            var validator = new UpdateWidgetModelValidator().Validate(model);

            if (!validator.IsValid)
            {
                return BadRequest(validator.Errors);
            }
            var result = await _widgetService.UpdateWidget(id,model);
            return Ok(result);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _widgetService.DeleteWidget(id);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Produces(typeof(WidgetDto))]
        public async Task<IActionResult> GetWidget(string id)
        {
            var result = _widgetService.GetWidget(id);
            return Ok(result);
        }
    }
}
