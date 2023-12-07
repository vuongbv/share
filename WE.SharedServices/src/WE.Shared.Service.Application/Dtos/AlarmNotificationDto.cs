using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WE.SharedService.Application.Dtos
{

    [BsonIgnoreExtraElements]
    public class AlarmNotificationDto
    {
        public string Severity { get; set; }
        public DateTime CreationTime { get; set; }
        public int Count { get; set; }
        public History History { get; set; }
        public DeviceInfo Source { get; set; }
        public string Type { get; set; }
        public DateTime Time { get; set; }
        public string Text { get; set; }
        public string Status { get; set; }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
    

    public class AuditRecords
    {

    }
    public class History
    {
        public AuditRecords AuditRecords { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class DeviceInfo
    {
        public string Name { get; set; }
        [BsonElement("_id")]
        public string Id { get; set; }
    }
}
