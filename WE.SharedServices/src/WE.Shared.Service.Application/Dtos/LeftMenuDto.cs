using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WE.SharedService.Application.Models.Panel;
using WE.Shared.Service.Domain.Entities;

namespace WE.SharedService.Application.Dtos
{
    public class LeftMenuDto
    {
        public string Id { get; set; }
        public string GroupName { get; set; }
        public string AppName { get; set; }
        public string AppId { get; set; }
        public bool IsHomePage { get; set; }
        public List<PanelDto> Panels { get; set; }
        public string? DeviceId { get; set; } = string.Empty;
    }

    public class PanelDto
    {
        public string Id { get; set; }
        public string PanelName { get; set; }
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public IEnumerable<WidgetDto> Widgets { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public string? DeviceId { get; set; } = string.Empty;
        public string AppName { get; set; }
        public List<Tab>? Tabs { get; set; }
    }


    public class WidgetDto
    {
        public string Id { get; set; }
        public string WidgetName { get; set; }
        public string? WidgetDescription { get; set; }
        public string? WidgetUnderTitleDescription { get; set; }
        public string? WidgetDataIcon { get; set; }
        public WidgetType WidgetType { get; set; }
        public ChartType? ChartType { get; set; }
        public string? SourceId { get; set; }
        public string? RefSourceId { get; set; }
        public string? DataPoint { get; set; }
        public string? DataPath { get; set; }
        public string? SubId { get; set; }
        public string? Unit { get; set; }
        public string? LineName { get; set; }
        public string? PinNumber { get; set; }
        public string PanelId { get; set; }
        public string? WorkspaceId { get; set; }
        public DataAggregateType DataAggregateType { get; set; }
        public DataResultType DataResultType { get; set; }
        public PowerType? PowerType { get; set; }
        public Periodicity? Periodicity { get; set; }
        public WidgetPosition Position { get; set; }
        public bool? IsHidden { get; set; } = false;
        public string? HtmlTemplate { get; set; }
        public string? TabName { get; set; }
        public WidgetSetting? Setting { get; set; }
        public string? DataFrequency { get; set; }
    }
}
