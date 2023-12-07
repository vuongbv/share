using Common.ControllerBase;
using Common.Mongodb.Collections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WE.SharedService.Application.Dtos;
using WE.SharedService.Application.Models.Groups;
using WE.SharedService.Application.Models.Tenant;
using WE.SharedService.Application.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using System.Collections.Generic;

namespace WE.Shared.Service.Controllers
{
    [Route("{tenant_name}/[controller]")]
    [ApiController]
    public class GroupsController : ApiControllerBase
    {
        public IGroupService _groupService;
        public IUserService _userService;
        public GroupsController(IGroupService groupService, IUserService userService)
        {
            _groupService = groupService;
            _userService = userService;
        }
        public string UserName => this.User.FindFirstValue("emails");
        [HttpGet("Search")]
        [SwaggerResponse(200, type: typeof(PagedList<GroupDto>))]
        public async Task<IActionResult> Search([FromQuery] SearchGroupViewModel search)
        {
            var result = await _groupService.Seach(search);
            return Ok(result);
        }

        [HttpGet("LeftMenu")]
        [SwaggerResponse(200, type: typeof(List<LeftMenuDto>))]
        public async Task<IActionResult> GetLeftMenu()
        {
            var user = await _userService.GetUser(UserName);
            if (string.IsNullOrEmpty(user.TenantName))
                return Unauthorized();
            var result = await _groupService.GetLeftMenu(user.Roles);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [SwaggerResponse(200, type: typeof(GroupDto))]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _groupService.GetById(id);
            return Ok(result);
        }

        [HttpPost]
        [Produces(typeof(string))]
        public async Task<IActionResult> Create([FromBody] GroupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _groupService.Create(model);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] GroupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _groupService.Update(id, model);
            return Ok();
        }
    }
}

class DevicesDto
{
    public string Id { get; set; }
    public string  Name { get; set; }
}