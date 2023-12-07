using WE.SharedService.Application.Dtos;
using WE.Shared.Service.Domain.Entities;

namespace WE.SharedService.Application.Models.Widgets;

public class WidgetBaseModel
{

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
    public string? WorkspaceId { get; set; }
    public DataAggregateType? DataAggregateType { get; set; }
    public DataResultType? DataResultType { get; set; }
    public string PanelId { get; set; }
    public Periodicity? Periodicity { get; set; }
    public PowerType? PowerType { get; set; }
    public WidgetPosition? Position { get; set; }
    public bool? IsHidden { get; set; } = false;
    public string? HtmlTemplate { get; set; }
    public string? TabName { get; set; }
    public WidgetSetting? Setting { get; set; }
    public string? DataFrequency { get; set; }
}
