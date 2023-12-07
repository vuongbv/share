using Common.MongoDb.Entities;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WE.Shared.Service.Domain.Entities
{
    public class Setting : BaseEntity
    {
        public string Key { get; set; } = default!;
        public BsonValue Value { get; set; } = default!;
    }
    public class WidgetTemplate
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
}
