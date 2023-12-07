using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WE.SharedService.Application.Dtos;

public class AlarmAuditDto
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string DeviceId { get; set; }
    public string DeviceName { get; set; }
    public string PreiousStatus { get; set; }
    public string NewStatus { get; set; }
    public string AlarmMessage { get; set; }
    public DateTimeOffset TimeUpdated { get; set; }
    public string Type { get; set; }
    public string CollectionName { get; set; }

}


