using Common.MongoDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WE.Shared.Service.Domain.Entities
{
    public class Panel : BaseEntity
    {
        public string PanelName { get; set; }
        public string GroupId { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public string? DeviceId { get; set; }
        public List<Tab>? Tabs { get; set; }
    }
    public class Tab
    {
        public string? Name { get; set; }
        public int? Priority { get; set; }
        public string? Id { get; set; } = Guid.NewGuid().ToString();
    }
}
