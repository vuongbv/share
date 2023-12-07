using System.Security.Claims;
using Common.ControllerBase;
using Common.Identity.Users;
using Common.Models;
using Common.Mongodb.Collections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using WE.SharedService.Application.Dtos;
using WE.SharedService.Application.Models.Users;
using WE.SharedService.Application.Services;

namespace WE.Shared.Service.Controllers
{
    [Route("{tenant_name}/[controller]")]
    [ApiController]
    public class UserController : ApiControllerBase
    {
        public UserController(
            IUserService userService
            )
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public string Email => this.User.FindFirstValue("emails");

        public IUserService _userService;


        [HttpGet("getInfo")]
        [Produces(typeof(UserDto))]
        public async Task<IActionResult> GetInfo()
        {
            try
            {
                var user = await _userService.GetUser(Email);
                if (string.IsNullOrEmpty(user.TenantName))
                    return Unauthorized();
                return Ok(user);
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Produces(typeof(UserDto))]
        public async Task<IActionResult> Get(string id)
        {
            var user = await _userService.GetUserById(id);

            return Ok(user);
        }

        [HttpGet("search")]
        [Produces(typeof(PagedList<UserDto>))]
        public async Task<IActionResult> Search([FromQuery] UserSearchViewModel search)
        {
            return Ok(await _userService.Search(search));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, UserViewModel model)
        {
            await _userService.Update(id, model);
            return Ok();
        }

        [HttpPost("assign-tenant/{id}")]
        public async Task<IActionResult> AssignTenant(string id, string tenantId)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("userId cannot be null or empty!");
            }

            if (string.IsNullOrEmpty(tenantId))
            {
                return BadRequest("tenantId cannot be null or empty!");
            }

            return Ok(await _userService.AssignTenant(id, tenantId));
        }

        [HttpPost()]
        public async Task<IActionResult> Create(CreateUserModel model)
        {
            var dto = await _userService.CreateUser(model);

            if (dto == null)
            {
                return NoContent();
            }

            return Ok(dto);
        }

        [HttpPost("end-user-session")]
        public async Task<IActionResult> EndUserSession(EndUserSessionModel model)
        {
            await _userService.EndSessionUser(model);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(string id)
        {
            var result = await _userService.Delete(id);
            if (result)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpGet("listroles")]
        [Produces(typeof(List<Role>))]
        public async Task<IActionResult> GetRoles()
        {
            var user = await _userService.GetRoles();

            return Ok(user);
        }


    }

}
