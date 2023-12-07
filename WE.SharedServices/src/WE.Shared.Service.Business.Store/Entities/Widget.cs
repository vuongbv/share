using Common.MongoDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WE.Shared.Service.Domain.Entities
{
    public class Widget : BaseEntity
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
        public string?SubId { get; set; }
        public string? Unit { get; set; }
        public string? LineName { get; set; }
        public string? PinNumber { get; set; }
        public string? WorkspaceId { get; set; }
        public DataAggregateType DataAggregateType { get; set; }
        public DataResultType DataResultType { get; set; }
        public string PanelId { get; set; }
        public PowerType? PowerType { get; set; }
        public Periodicity? Periodicity { get; set; }
        public WidgetPosition Position { get; set; }
        public bool IsHidden { get; set; }
        public string? HtmlTemplate { get; set; }
        public string? TabName { get; set; }
        public WidgetSetting? Setting { get; set; }
        public string? DataFrequency { get; set; }
    }
    public class WidgetSetting
    {
        public int? DecimalSetting { get; set; }
        public List<StyleSetting>? StyleSettingList { get; set; }
        public List<WidgetAdditionalDataList>? WidgetAdditionalDataList { get; set; }
        public string? WidgetPowerBIFilterSetting { get; set; }
        public string? ChartStyleSettingId { get; set; }
        public string? Formular { get; set; }

    }
    public class StyleSetting
    {
        public double? ValueFrom { get; set; }
        public double? ValueTo { get; set; }
        public string? ValueEqual { get; set; }
        public Operator? Operator { get; set; }
        public string Color { get; set; }

    }
    public class WidgetAdditionalDataList
    {
        public string? IndicatorName { get; set; }
        public string? SourceId { get; set; }
        public string? DataPoint { get; set; }
        public string? DataPath { get; set; }
        public string? SubId { get; set; }
        public string? Unit { get; set; }
        public string? Color { get; set; }

    }
    public enum Operator
    {
        range,
        range_equal,
        eq,
        neq
    }
    public enum Periodicity
    {
        daily,
        weekly,
        monthly,
        yearly,

    }
    public enum WidgetType
    {
        Chart,
        PowerBI,
        Widget,
        Map,
        DataList,
        Weather,
        DeviceStatus,
        IFrame,
        SingleValue,
        SingleChart,
        Html,
        MultiChart,
        MultiValue,
        Power,
        PowerBatch
    }
    public enum ChartType
    {
        Line,
        Bar,
        Gauge,
        Bullet,
        Signal,
        SignalData,
        Battery
    }
    public enum DataAggregateType
    {
        Min,
        Max,
        Avg,
        Sum,
        Current
    }

    public enum DataResultType
    {
        Record,
        RecordList,
        Hourly,
        Daily,
        Weekly,
        Monthly,
        Yearly
    }
    public enum PowerType
    {
        Power,
        Current,
        Voltage,
        Money
    }
    public class WidgetPosition
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

}
