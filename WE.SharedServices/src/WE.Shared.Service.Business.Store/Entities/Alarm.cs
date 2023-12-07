using Common.MongoDb.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WE.Shared.Service.Domain.Entities
{
    public class Alarm: BaseEntity
    {
        public DeviceAlarm Source { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        public string Severity { get; set; }
        public string Status { get; set; }
        public DateTime Time { get; set; }
    }

    public class DeviceAlarm
    {
        public string Name { get; set; }
        public string Id { get; set; }
    }
}
