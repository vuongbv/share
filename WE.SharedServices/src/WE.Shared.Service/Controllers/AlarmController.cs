using AutoMapper;
using Common.ControllerBase;
using Common.Mongodb.Collections;
using Microsoft.AspNetCore.Mvc;
using WE.SharedService.Application.Dtos;
using Microsoft.AspNetCore.SignalR;
using WE.SharedService.Application.Models.Alarm;
using WE.SharedService.Application.Services;
using WE.Shared.Service.SignalrRHub;
using Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using WE.SharedService.Application.Constants;
using WE.Shared.Service.Domain.Entities;

namespace WE.Shared.Service.Controllers
{
    [ApiController]
    [Route("{tenant_name}/[controller]")]
    public class AlarmController : ApiControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAlarmService _alarmService;
        private IHubContext<AlarmHub> _alarmHubContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private string tenantName;

        public AlarmController(IMapper mapper,
            IAlarmService alarmService,
            IHubContext<AlarmHub> alarmHubContext,
            IHttpContextAccessor httpContextAccessor
            )
        {
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._alarmService = alarmService ?? throw new ArgumentNullException(nameof(alarmService));
            this._alarmHubContext = alarmHubContext ?? throw new ArgumentNullException(nameof(alarmHubContext));
            this._httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            this.tenantName = _httpContextAccessor.HttpContext.GetRouteData().Values["tenant_name"] as string ?? string.Empty;
        }

        [HttpGet()]
        [Produces(typeof(PagedList<AlarmDto>))]
        public async Task<IActionResult> GetAll()
        {

            var result = (await _alarmService.GetAll());
            return Ok(new { alarms = result });
        }


        [HttpPost()]
        public async Task<IActionResult> Create([FromBody]CreateAlarmModel model)
        {
            var validator = (new CreateAlarmModelValidator()).Validate(model);

            if (!validator.IsValid)
            {
                return BadRequest(validator.Errors);
            }

            var result = await _alarmService.Create(model);

            var realTimeDto = new AlarmRealTimeDto()
            {
                DeviceId = model.Source.Id,
                Id = null,
                AlarmMessage = model.Text,
                PanelId = "Panel-01",
                Severity = model.Severity,
            };

            await SendRealTimeData(new List<AlarmRealTimeDto> { realTimeDto });

            return Ok(result);
        }

        private async Task SendRealTimeData(List<AlarmRealTimeDto> dto)
        {
            await _alarmHubContext.Clients.Group(AlarmHub.AlarmGroup + this.tenantName).SendAsync(AlarmHub.AlarmUpdated, dto);
        }


        [HttpGet("search")]
        [Produces(typeof(PagedList<AlarmDto>))]
        public async Task<IActionResult> Filter([FromQuery] SearchAlarmModel model)
        {
            var result = await _alarmService.Search(model);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody]UpdateAlarmModel model)
        {
            var validator = (new UpdateAlarmModelValidator()).Validate(model);

            if (!validator.IsValid)
            {
                return BadRequest(validator.Errors);
            }

            var result = await _alarmService.Update(id, model);

            if (result)
            {
                await SendAlarmToRealTime(new List<string> { id });
                
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPut("{ids}/{status}")]
        public async Task<IActionResult> UpdateStatus(string ids, string status)
        {
            var listIds = ids.Split(',').ToList();
            if(listIds.Count > 0)
            {
                switch (status.ToLower())
                {
                    case AlarmConstant.Acknowledged:
                        {
                            
                            await _alarmService.UpdateAlarmStatus(listIds, AlarmConstant.Acknowledged);

                            //real time update
                            await SendAlarmToRealTime(listIds);

                            return Ok();
                        }
                    case AlarmConstant.Resolved:
                        {
                            await _alarmService.UpdateAlarmStatus(listIds, AlarmConstant.Resolved);

                            //real time update
                            await SendAlarmToRealTime(listIds);
                            return Ok();
                        }
                    default:
                        break;
                }
            }
            return BadRequest();
        }

        private async Task SendAlarmToRealTime(List<string> listAlarmIds, bool isCreateAlarm = false)
        {
            //real time update
            var listAlarm = await _alarmService.GetAlarmByIds(listAlarmIds);
            var listRealTimeDto = new List<AlarmRealTimeDto>();

            foreach (var alarm in listAlarm)
            {
                var realTimeDto = new AlarmRealTimeDto()
                {
                    DeviceId = alarm.Source.Id,
                    Id = alarm.Id,
                    AlarmMessage = isCreateAlarm ? alarm.Text : null,
                    PanelId = "Panel-01",
                    Severity = alarm.Severity,
                };
                listRealTimeDto.Add(realTimeDto);
            }

            await SendRealTimeData(listRealTimeDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _alarmService.Delete(id);
            await SendAlarmToRealTime(new List<string> { id });
            return Ok();
        }

        [HttpGet("Device")]
        [Produces(typeof(PagedList<AlarmNotificationDto>))]
        public async Task<IActionResult> GetAlarmByDevice(string? id, [FromQuery] SearchAlarmModel model)
        {
            var result = await _alarmService.GetAlarmByDeviceIdUsingDatabaseLayer(id, model);

            var paged = await result.AsQueryable().ToPagedList(model);

            return Ok(paged);
        }

    }
}
