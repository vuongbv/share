using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Common.Mongodb.Collections;
using MongoDB.Driver;
using WE.Shared.Service.Domain.Entities;
using WE.Shared.Service.Domain.Repository;
using WE.SharedService.Application.Dtos;
using WE.SharedService.Application.Models.Groups;
using Common.Identity.Users;
using WE.SharedService.Application.Constants;
using MongoDB.Bson.Serialization;
using Org.BouncyCastle.Utilities;
using WE.SharedService.Application.Models.TenantApplication;
using MongoDB.Bson;
using System.Globalization;
using WE.SharedService.Application.Models.Keyword;
using System.Linq.Expressions;

namespace WE.SharedService.Application.Services
{
    public interface IGroupService : IBaseServices<GroupViewModel, GroupDto, SearchGroupViewModel>
    {

    }
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IDevicesRepository _devicesRepository;
        private readonly IPanelRepository _panelRepository;
        private readonly IWidgetRepository _widgetManagementRepository;
        private readonly ITenantSettingRepository _tenantSettingRepository;
        private readonly IApplicationNameRepository _applicationNameRepository;
        private readonly ISettingRepository _settingRepository;

        readonly  IMapper _mapper;
        public GroupService(IGroupRepository groupRepository, IMapper mapper, IDevicesRepository devicesRepository, IPanelRepository panelRepository, IWidgetRepository widgetManagementRepository, ITenantSettingRepository tenantSettingRepository, IApplicationNameRepository applicationNameRepository, ISettingRepository settingRepository)
        {
            _groupRepository = groupRepository;
            _mapper = mapper;
            _devicesRepository = devicesRepository;
            _panelRepository = panelRepository;
            _widgetManagementRepository = widgetManagementRepository;
            _tenantSettingRepository = tenantSettingRepository;
            _applicationNameRepository = applicationNameRepository;
            _settingRepository = settingRepository;
        }

        public async Task<PagedList<GroupDto>> Seach(SearchGroupViewModel searchModel)
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
            var query = _groupRepository.GetAllWithDataAuthorize();
            var devices = _devicesRepository.GetAll().ToList();

            if (!string.IsNullOrEmpty(searchModel.Name))
            {
                query = query.Where(x => x.Name.Contains(searchModel.Name));
            }

            var group = _mapper.Map<PagedList<GroupDto>>(await query.ToPagedList(searchModel));

            group.Items = group.Items.Select(i => new GroupDto()
            {
                Id = i.Id,
                Name = i.Name,
                AppName = (lstAppName.First(a => a.Id.Equals(i.AppId)) != null ? lstAppName.First(a => a.Id.Equals(i.AppId)).Name : lstAppName.First(a => a.Id.Equals(dfApplication)).Name),
                DevicesName = devices.Where(n => i.DeviceIds.Contains(n.Id)).Select(m => m.Name),
                Devices = _mapper.Map<IEnumerable<DeviceDto>>(devices.Where(n => i.DeviceIds.Contains(n.Id)))
            }).Where(item =>
            (string.IsNullOrEmpty(searchModel.Name) || WildcardMatch(item.Name, searchModel.Name)) &&
            (string.IsNullOrEmpty(searchModel.ApplicationName) || WildcardMatch(item.AppName, searchModel.ApplicationName))
            ).ToList();

            var paggedResult = new PagedList<GroupDto>(group.Items.AsQueryable(), searchModel);

            return group;
        }

        private static bool WildcardMatch(string text, string pattern)
        {
            pattern = pattern.Replace("*", ".*");
            return System.Text.RegularExpressions.Regex.IsMatch(text, pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }


        public async Task<List<LeftMenuDto>> GetLeftMenu(IList<Role> roles)
        {
            var lstAppName = _applicationNameRepository.GetAll().ToList();
            var config = (await _tenantSettingRepository.FindAsync(x => x.Key == TenantSettingKeyConstants.TenantApplicationKey)).FirstOrDefault();
            var keywords = (await _settingRepository.FindAsync(x => x.Key == TenantSettingKeyConstants.Keyword)).FirstOrDefault();
            var lstapp = new List<string>();
            var dfApplication = string.Empty;
            if (config != null)
            {
                var application = BsonSerializer.Deserialize<TenantApplication>(config.Value.ToJson());
                lstapp = application.Application;
                dfApplication = application.DefaultApplication;
            }
            var groups = _groupRepository.GetAll().ToList().Where(g => !lstapp.Any() || g.AppId == null || lstapp.Any(a => a == g.AppId)).Where(x => x.Roles == null || (x.Roles.Any() && x.Roles.Any(r => roles.Any(ri => ri.Name.Equals(r))))).ToList();
            var panels = _panelRepository.GetAll().ToList().Where(x => x.Roles == null || (x.Roles.Any() && x.Roles.Any(r => roles.Any(ri => ri.Name.Equals(r))))).ToList();

            var widgets = _widgetManagementRepository.GetAll().ToList();

            var listDeviceIds = widgets.Select(x => x.SourceId);

            var activeDevice = _devicesRepository.GetAll().Where(x => x.Status == null || x.Status == DeviceConstants.ActiveDevice).Select(x => x.SourceId).ToList();

            var listActiveDeviceId = activeDevice.Where(x => activeDevice.Contains(x)).ToList();

            var guid = Guid.Empty;

            var results = groups.Select(g => new LeftMenuDto()
            {
                GroupName = g.Name,
                Id = g.Id,
                AppName = (lstAppName.First(a => a.Id.Equals(g.AppId)) != null ? lstAppName.First(a => a.Id.Equals(g.AppId)).Name : lstAppName.First(a => a.Id.Equals(dfApplication)).Name),
                AppId = g.AppId,
                IsHomePage = g.IsHomePage,
                Panels = panels.Where(x => x.GroupId.Equals(g.Id)).Select(p => new PanelDto()
                {
                    PanelName = p.PanelName,
                    Id = p.Id,
                    DeviceId = p.DeviceId,
                    Tabs = p.Tabs,
                    //--fix #17722, modified to support comma delimited widget sourceids--//
                    //Widgets = widgets.Where(w => w.PanelId.Equals(p.Id) && !w.IsHidden && (w.WidgetType == WidgetType.Html || w.WidgetType == WidgetType.IFrame || w.WidgetType == WidgetType.Map || string.IsNullOrEmpty(w.SourceId) || listActiveDeviceId.Contains(w.SourceId) || Guid.TryParse(w.SourceId, out guid))).Select(w => new WidgetDto()
                    Widgets = widgets.Where(w => w.PanelId.Equals(p.Id) && !w.IsHidden && (w.WidgetType == WidgetType.Html || w.WidgetType == WidgetType.IFrame || w.WidgetType == WidgetType.Map || 
                        string.IsNullOrEmpty(w.SourceId) || w.SourceId.Split(',').ToList().Any(cc => listActiveDeviceId.Any(dd => cc == dd)) || 
                        Guid.TryParse(w.SourceId, out guid))).Select(w => new WidgetDto()
                    {
                        Id = w.Id,
                        DataAggregateType = w.DataAggregateType,
                        DataPoint = w.DataPoint,
                        DataResultType = w.DataResultType,
                        SourceId = w.SourceId,
                        RefSourceId = w.RefSourceId,
                        WidgetName = DynamicConvert(w.WidgetName, keywords, panels, w.PanelId),
                        WidgetType = w.WidgetType,
                        WorkspaceId = w.WorkspaceId,
                        Unit = w.Unit,
                        LineName = w.LineName,
                        HtmlTemplate = w.HtmlTemplate,
                        PinNumber = w.PinNumber,
                        SubId = w.SubId,
                        PowerType = w.PowerType,
                        ChartType = w.ChartType,
                        Periodicity = w.Periodicity,
                        DataPath = w.DataPath,
                        Position = w.Position,
                        IsHidden = w.IsHidden,
                        TabName = w.TabName != null ? ((p.Tabs != null && p.Tabs.Any(x => x.Id == w.TabName)) ? p.Tabs?.First(x => x.Id == w.TabName).Id.ToLower() : String.Empty) : String.Empty,
                        Setting = w.Setting,
                        WidgetDescription = DynamicConvert(w.WidgetDescription, keywords, panels, w.PanelId),
                        WidgetUnderTitleDescription = DynamicConvert(w.WidgetUnderTitleDescription, keywords, panels, w.PanelId),
                        WidgetDataIcon = w.WidgetDataIcon,
                        DataFrequency = w.DataFrequency
                    })
                }).ToList().ToList().OrderBy(x => x.PanelName).ToList()
            }).ToList();

            return results;
         
        
        }

        public async Task<string> Create(GroupViewModel model)
        {
            var config = (await _tenantSettingRepository.FindAsync(x => x.Key == TenantSettingKeyConstants.TenantApplicationKey)).FirstOrDefault();
            var lstapp = new List<string>();
            var dfApplication = string.Empty;
            if (config != null)
            {
                var application = BsonSerializer.Deserialize<TenantApplication>(config.Value.ToJson());
                lstapp = application.Application;
                dfApplication = application.DefaultApplication;
            }
            if (string.IsNullOrEmpty(model.AppId))
            {
                model.AppId = dfApplication;
            }
            var group = _mapper.Map<Group>(model);
            var result = await _groupRepository.InsertAsync(group);
            return result.Id;
        }

        public async Task Update(string id, GroupViewModel model)
        {
            var modelUpdate = _groupRepository.GetFirstOrDefault(i => i.Id.Equals(id));
            _mapper.Map(model, modelUpdate);
            await _groupRepository.UpdateAsync(modelUpdate, x => x.Id == id);
        }


        public async Task<GroupDto> GetById(string id)
        {
            return _mapper.Map<GroupDto>(_groupRepository.GetFirstOrDefault(i => i.Id.Equals(id)));
        }

        public string? DynamicConvert(string? s, Setting? setting, List<Panel> panels, string panelId)
        {
            if(setting != null && !string.IsNullOrEmpty(s))
            {
                var lstKeywords = BsonSerializer.Deserialize<Keyword>(setting.Value.ToJson()).KeywordList.Select(x => x.Key);
                if(lstKeywords.Any(k => s.IndexOf(k) > -1))
                {
                    foreach(var keyword in lstKeywords)
                    {
                        s = s.Replace(keyword, ConvertKeyword(keyword, panels, panelId));
                    }
                }
            }
            return s;
        }
        public string ConvertKeyword(string keyword, List<Panel> panels, string panelId)
        {

            var result = string.Empty;
            switch (keyword)
            {
                case "[CurrentDay]":
                    result = DateTime.Now.DayOfWeek.ToString();
                    break;
                case "[CurrentMonth]":
                    result = DateTime.Now.ToString("MMMM");
                    break;
                case "[CurrentYear]":
                    result = DateTime.Now.ToString("yyyy");
                    break;
                case "[CurrentDateTime]":
                    //disable as left menu does not refresh many time
                    //result = DateTime.Now.ToString("dd-MM-yyyy HH:mm");
                    result = keyword;
                    break;
                case "[CurrentDayShort]":
                    result = DateTime.Now.ToString("ddd");
                    break;
                case "[CurrentMonthShort]":
                    result = DateTime.Now.ToString("MMM"); ;
                    break;
                case "[PrevDay]":
                    result = DateTime.Now.AddDays(-1).DayOfWeek.ToString();
                    break;
                case "[PrevMonth]":
                    result = DateTime.Now.AddMonths(-1).ToString("MMMM");
                    break;
                case "[PrevYear]":
                    result = DateTime.Now.AddYears(-1).ToString("yyyy");
                    break;
                case "[PrevDateTime]":
                    //disable as left menu does not refresh many time
                    //result = DateTime.Now.AddDays(-1).ToString("dd-MM-yyyy HH:mm");
                    result = keyword;
                    break;
                case "[PrevDayShort]":
                    result = DateTime.Now.AddDays(-1).ToString("ddd");
                    break;
                case "[PrevMonthShort]":
                    result = DateTime.Now.AddMonths(-1).ToString("MMM");
                    break;
                case "[DeviceName]":
                    {
                        var panel = panels.FirstOrDefault(x => x.Id == panelId);
                        if(panel != null)
                        {
                            var deviceId = panel.DeviceId.Split(",").First();
                            var device = _devicesRepository.GetFirstOrDefault(x => x.SourceId.Equals(string.IsNullOrEmpty(deviceId) ? String.Empty : deviceId));
                            if(device != null)
                            {
                                result = device.Name;
                            }
                        }
                    }
                    break;
                case "[DeviceSourceId]":
                    {
                        var panel = panels.FirstOrDefault(x => x.Id == panelId);
                        if (panel != null && !string.IsNullOrEmpty(panel.DeviceId))
                        {
                            result = panel.DeviceId.Split(",").First();
                        }
                    }
                    break;
                case "[NextMonthShort]":
                    result = DateTime.Now.AddMonths(1).ToString("MMM");
                    break;
                case "[NextDayShort]":
                    result = DateTime.Now.AddDays(1).ToString("ddd");
                    break;
                case "[NextYear]":
                    result = DateTime.Now.AddYears(1).ToString("yyyy");
                    break;
                case "[NextMonth]":
                    result = DateTime.Now.AddMonths(1).ToString("MMMM");
                    break;
                case "[NextDay]":
                    result = DateTime.Now.AddDays(1).DayOfWeek.ToString();
                    break;
                case "[LastDayNumOfMonth]":
                    result = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month).ToString();
                    break;
                default:
                    // code block
                    break;
            }
            return result;
        }
    }
}
