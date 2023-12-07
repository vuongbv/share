using Common.ControllerBase;
using Common.Mongodb.Collections;
using Microsoft.AspNetCore.Mvc;
using WE.SharedService.Application.Dtos;
using WE.SharedService.Application.Models.Audit;
using WE.SharedService.Application.Services;

namespace WE.Shared.Service.Controllers;

[ApiController]
[Route("{tenant_name}/[controller]")]
public class AuditController : ApiControllerBase
{
    private readonly IAuditService _auditService;

    public AuditController(IAuditService auditService)
    {
        this._auditService = auditService ?? throw new ArgumentNullException(nameof(auditService));
    }

    [HttpPost("last-login")]
    public async Task<IActionResult> CreateLastLogin(CreateUserLastLoginModel model )
    {
        await _auditService.CreateLastUserLogin(model);
        return Ok();
    }

    [HttpGet("alarm")]
    [Produces(typeof(PagedList<AlarmAuditDto>))]
    public async Task<IActionResult> AlarmAudit([FromQuery]GetAlarmAuditModel model)
    {
        var result = await _auditService.GetAlarmAudit(model);

        if (result == null)
        {
            return NoContent();
        }


        return Ok(new PagedList<AlarmAuditDto>(result, model.PageSize, model.PageIndex));
    }

    [HttpGet("role")]
    [Produces(typeof(PagedList<RoleAuditDto>))]
    public async Task<IActionResult> RoleAudit([FromQuery] GetRoleAuditModel model)
    {
        var result = await _auditService.GetRoleAudit(model);
        if (result == null)
        {
            return NoContent();
        }
        var pagged = result.ToPagedList(model.PageIndex, model.PageSize, 1);
        return Ok(pagged);
    }

    [HttpGet("last-login")]
    [Produces(typeof(PagedList<LastLoginAuditDto>))]
    public async Task<IActionResult> LastLoginAudit([FromQuery] GetLastLoginModel model)
    {
        var result = await _auditService.GetLastLoginAudit(model);
        if (result == null)
        {
            return NoContent();
        }
        return Ok(new PagedList<LastLoginAuditDto>(result, model.PageSize, model.PageIndex));
    }

}