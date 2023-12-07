using WE.Shared.Service.Domain.Entities;

namespace WE.SharedService.Application.Models.WidgetTemplate;

public class WidgetTemplateBaseModel
{
    public string TemplateName { get; set; }
    public string WidgetName { get; set; }
    public string? WidgetDescription { get; set; }
    public string? WidgetDataIcon { get; set; }
    public WidgetType WidgetType { get; set; }
    public ChartType? ChartType { get; set; }
    public string DataPoint { get; set; }
    public DataAggregateType DataAggregateType { get; set; }
    public DataResultType DataResultType { get; set; }
    public Periodicity? Periodicity { get; set; }
    public PowerType? PowerType { get; set; }
}
