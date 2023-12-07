using Common.MongoDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WE.Shared.Service.Domain.Entities
{
    public class Device : BaseEntity
    {
        public string Name { get; set; }
        public string Owner { get; set; }
        public Position GT_Position { get; set; }
        public string SourceId { get; set; }
        public string Status { get; set; }
    }
    public class Position
    {
        public decimal Lng { get; set; }
        public decimal Lat { get; set; }
        public int Alt { get; set; }
    }
}
