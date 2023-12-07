using WE.Shared.Service.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WE.SharedService.Application.Models.Alarm
{
    public class AlarmBaseModel
    {
        public DeviceAlarmModel Source { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        public string Severity { get; set; }
        public string Status { get; set; }
        public DateTime Time { get; set; }
    }
    public class DeviceAlarmModel
    { 
        public string Name { get; set; }
        public string Id { get; set; }
    }

}
