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
    public class TenantSetting : BaseEntity
    {
        public double TariffValue { get; set; }
        public string? TariffUnit { get; set; }
        public DateTime? DataLastSync { get; set; }
        public string Key { get; set; } = default!;
        public BsonValue Value { get; set; } = default!;
    }
}
