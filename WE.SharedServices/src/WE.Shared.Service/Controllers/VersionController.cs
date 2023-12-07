using Microsoft.AspNetCore.Mvc;
using WE.SharedService.Application.Services;

namespace WE.Shared.Service.Controllers;


[ApiController]
[Route("[controller]")]
public class VersionController : ControllerBase
{
    private IVersionService _versionService;

    public VersionController(IVersionService versionService)
    {
        this._versionService = versionService;
    }

    [HttpGet]
    [Produces(typeof(PackageVersion))]
    public async Task<IActionResult> Version(string packageName)
    {
        var result = await _versionService.GetPackageVersion(packageName);
        return Ok(result);
    }
}
