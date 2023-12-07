using Common.ControllerBase;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Common.Mongodb.Collections;
using WE.SharedService.Application.Services;
using Swashbuckle.AspNetCore.Annotations;

using WE.SharedService.Application.Dtos;
using WE.SharedService.Application.Models.Tenant;
using Microsoft.AspNetCore.Authorization;

namespace WE.Shared.Service.Controllers;

[ApiController]
[Route("[controller]")]
public class TenantController : Controller
{
    private readonly IMapper _mapper;
    private readonly ITenantService _tenantService;

    public TenantController(IMapper mapper, ITenantService tenantService)
    {
        this._mapper = mapper;
        _tenantService = tenantService;
    }

    [HttpGet("Search")]
    [SwaggerResponse(200, type: typeof(PagedList<TenantDto>))]
    public async Task<IActionResult> Search([FromQuery] SearchTenantViewModel search)
    {
        var result = await _tenantService.Seach(search);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [SwaggerResponse(200, type: typeof(TenantDto))]
    public async Task<IActionResult> Get(string id)
    {
        var result = await _tenantService.GetById(id);
        return Ok(result);
    }

    [HttpPost]
    [Produces(typeof(TenantDto))]
    [AllowAnonymous]
    public async Task<IActionResult> CreateTenant([FromBody] TenantViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var result = await _tenantService.Create(model);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTenant(string id, [FromBody] TenantUpdateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await _tenantService.Update(id, model);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTenant(string id)
    {
        await _tenantService.Delete(id);
        return Ok();
    }

    [HttpGet("generate-api-key")]
    [Produces(typeof(PagedList<ApiKeyDto>))]
    public async Task<IActionResult> SearchKey([FromQuery] SearchApiKeyViewModel search)
    {
       
        var apiKey = await _tenantService.SearchGenerateApiKey(search);

        return Ok(apiKey);
    }

    [HttpPost("generate-api-key/{id}")]
    [Produces(typeof(string))]
    public async Task<IActionResult> GenerateApiKey(string id, [FromBody]CreateApiKeyModel model)
    {
        var generateModel = new GenerateApiKeyModel
        {
            ExpiryTime = model.ExpiryTime,
            Scopes = model.Scopes,
            TenantId = id
        };

        var apiKey = await _tenantService.GenerateApiKey(generateModel);

        return Ok(apiKey);
    }

    [HttpPost("generate-mqtt-client/{id}")]
    [Produces(typeof(string))]
    public async Task<IActionResult> GenerateMqttClient(string id, [FromBody] CreateMqttClientCredentialModel model)
    {
        var generateModel = new GenerateMqttClientCredentialModel
        {
            UserName = model.UserName,
            Password = model.Password,
            TenantId = id
        };

        var apiKey = await _tenantService.GenerateMqttClient(generateModel);

        return Ok(apiKey);
    }

    [HttpGet("generate-api-key/{id}")]
    [Produces(typeof(ApiKeyDto))]
    public async Task<IActionResult> GetGenerateApiKey(string id)
    {

       var key =   await _tenantService.GetGenerateApiKey(id);

        return Ok(key);
    }
    [HttpDelete("generate-api-key/{id}")]
    
    public async Task<IActionResult> Delete(string id)
    {
         await _tenantService.DeleteGenerateApiKey(id);

        return Ok();
    }

}

