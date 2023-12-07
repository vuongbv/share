using Common.ControllerBase;
using Common.Mongodb.Collections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using WE.SharedService.Application.Constants;
using WE.SharedService.Application.Dtos;
using WE.SharedService.Application.Models.Panel;
using WE.SharedService.Application.Models.TenantApplication;
using WE.SharedService.Application.Services;
using WE.Shared.Service.Domain.Repository;

namespace WE.Shared.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PanelController : ApiControllerBase
    {
        private readonly IPanelService _panelService;
        private readonly IGroupRepository _groupRepository;
        private readonly IApplicationNameRepository _applicationNameRepository;
        private readonly ITenantSettingRepository _tenantSettingRepository;

        public PanelController(IPanelService panelService, IGroupRepository groupRepository, IApplicationNameRepository applicationNameRepository, ITenantSettingRepository tenantSettingRepository)
        {
            this._panelService = panelService ?? throw new ArgumentNullException(nameof(panelService));
            _groupRepository = groupRepository;
            _applicationNameRepository = applicationNameRepository;
            _tenantSettingRepository = tenantSettingRepository;
        }

        [HttpGet]
        [Produces(typeof(PagedList<PanelDto>))]
        public async Task<IActionResult> GetAll([FromQuery] SearchPanelViewModel searchModel)
        {
            var lstAppName = _applicationNameRepository.GetAll().ToList();
            var config = (await _tenantSettingRepository.FindAsync(x => x.Key == TenantSettingKeyConstants.TenantApplicationKey)).FirstOrDefault();
            var lstapp = new List<string>();
            var dfApplication = string.Empty;

            if (config != null)
            {
                var application = BsonSerializer.Deserialize<TenantApplication>(config.Value.ToJson());
                lstapp = application.Application;
                dfApplication = application.DefaultApplication;
            }
            var groupList = _groupRepository.GetAll().ToList();

            var result = await this._panelService.GetAllPanel(searchModel);
            var listResult = await this._panelService.GetAllPanel();

            listResult = listResult.Join(groupList, i => i.GroupId, g => g.Id, (i, g) => new PanelDto
            {
                DeviceId = i.DeviceId,
                Id = i.Id,
                PanelName = i.PanelName,
                Widgets = i.Widgets,
                Roles = i.Roles,
                GroupId = i.GroupId,
                GroupName = g.Name,
                AppName = (lstAppName.First(a => a.Id.Equals(g.AppId)) != null ? lstAppName.First(a => a.Id.Equals(g.AppId)).Name : lstAppName.First(a => a.Id.Equals(dfApplication)).Name)
            }).Where(item =>
            (string.IsNullOrEmpty(searchModel.Name) || WildcardMatch(item.PanelName, searchModel.Name)) &&
            (string.IsNullOrEmpty(searchModel.ApplicationName) || WildcardMatch(item.AppName, searchModel.ApplicationName))

        ).ToList();

            var paggedResult = new PagedList<PanelDto>(listResult.AsQueryable(), searchModel);

            return Ok(paggedResult);
        }

        private static bool WildcardMatch(string text, string pattern)
        {
            pattern = pattern.Replace("*", ".*");
            return System.Text.RegularExpressions.Regex.IsMatch(text, pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }


        [HttpGet("{id}")]
        [Produces(typeof(PanelDto))]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _panelService.GetPanel(id);

            return Ok(result);
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] CreatePanelModel model)
        {
            var validator = new CreatePanelModelValidator().Validate(model);

            if (!validator.IsValid)
            {
                return BadRequest(validator.Errors);
            }

            var result = await _panelService.CreatePanel(model);

            return Ok(result);
        }

        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Update(string id, [FromBody] UpdatePanelModel model)
        {
            var validator = new UpdatePanelModelValidator().Validate(model);

            if (!validator.IsValid)
            {
                return BadRequest(validator.Errors);
            }

            var result = await _panelService.UpdatePanel(id, model);
            return Ok(result);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _panelService.DeletePanel(id);
            return Ok(result);
        }
    }
}
