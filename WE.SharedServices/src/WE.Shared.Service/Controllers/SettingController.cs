using Common.ControllerBase;
using Common.Mongodb.Collections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using WE.SharedService.Application.Constants;
using WE.SharedService.Application.Dtos;
using WE.SharedService.Application.Models.Keyword;
using WE.SharedService.Application.Models.WidgetTemplate;
using WE.SharedService.Application.Services;
using WE.Shared.Service.Domain.Entities;

namespace WE.Shared.Service.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class SettingController : ApiControllerBase
    {
        private readonly ISettingService _settingService;

        public SettingController(ISettingService settingService)
        {
            this._settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
        }

        [HttpGet("widgettemplate")]
        [Produces(typeof(List<WidgetTemplateDto>))]
        public async Task<IActionResult> GetAll([FromQuery]string? settingType)
        {
            var result = (await this._settingService.GetSettingByType(settingType)).Select(x => new WidgetTemplateDto { Id = x.Id , Template = BsonSerializer.Deserialize<WidgetTemplate>(x.Value.ToJson()) }).ToList();

            return Ok(result);
        }

        [HttpPost("widgettemplate")]
        [Produces(typeof(WidgetTemplateDto))]
        public async Task<IActionResult> Create([FromBody] CreateWidgetTemplateModel model)
        {
            var validator = new CreateWidgetTemplateModelValidator().Validate(model);

            if (!validator.IsValid)
            {
                return BadRequest(validator.Errors);
            }
            var data = new Setting
            {
                UpdatedDate = DateTime.UtcNow,
                Key = TenantSettingKeyConstants.WidgetTemplate,
                Value = model.ToBsonDocument(),
                CreatedDate = DateTime.UtcNow
            };
            var result = await _settingService.CreateSetting(data);

            return Ok(new WidgetTemplateDto
            {
                Id = result.Id,
                Template = BsonSerializer.Deserialize<WidgetTemplate>(result.Value.ToJson())
            });
        }

        [HttpPut("widgettemplate/{id}")]
        [Produces(typeof(WidgetTemplateDto))]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateWidgetTemplateModel model)
        {
            var validator = new UpdateWidgetTemplateModelValidator().Validate(model);

            if (!validator.IsValid)
            {
                return BadRequest(validator.Errors);
            }
            var data = new Setting
            {
                UpdatedDate = DateTime.UtcNow,
                Value = model.ToBsonDocument()
            };
            var result = await _settingService.UpdateSetting(id, data);
            return Ok(result);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _settingService.DeleteSetting(id);
            return Ok(result);
        }

        [HttpGet("widgettemplate/{id}")]
        [Produces(typeof(WidgetTemplateDto))]
        public async Task<IActionResult> GetSetting(string id)
        {
            var result = await _settingService.GetSetting(id);
            if(result != null)
            {
                return Ok(new WidgetTemplateDto
                {
                    Id = result.Id,
                    Template = BsonSerializer.Deserialize<WidgetTemplate>(result.Value.ToJson())
                });
            }
            return BadRequest();
        }

        //Dynamic widget header

        [HttpPost("keyword")]
        [Produces(typeof(KeywordDto))]
        public async Task<IActionResult> CreateKeywork([FromBody] List<KeywordModel> model)
        {
            var data = new Setting
            {
                UpdatedDate = DateTime.UtcNow,
                Key = TenantSettingKeyConstants.Keyword,
                Value = (new Keyword { KeywordList = model }).ToBsonDocument(),
                CreatedDate = DateTime.UtcNow
            };
            var result = await _settingService.CreateSetting(data);

            return Ok(new KeywordDto
            {
                Id = result.Id,
                Keys = BsonSerializer.Deserialize<Keyword>(result.Value.ToJson())
            });
        }


        [HttpGet("keyword")]
        [Produces(typeof(List<KeywordDto>))]
        public async Task<IActionResult> GetKeywordSetting([FromQuery] string? settingType)
        {
            var results = (await this._settingService.GetSettingByType(settingType)).Select(x => new KeywordDto { Id = x.Id, Keys = BsonSerializer.Deserialize<Keyword>(x.Value.ToJson()) }).ToList();
            if (results.Any())
            {
                return Ok(results.FirstOrDefault());
            }
            return BadRequest();
        }
    }
}
