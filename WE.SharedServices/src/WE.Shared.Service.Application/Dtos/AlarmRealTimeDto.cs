using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WE.SharedService.Application.Dtos;

public class AlarmRealTimeDto
{
    public string Id { get; set; }
    public string DeviceId { get; set; }
    public string AlarmMessage { get; set; }
    public string PanelId { get; set; }
    public string Severity { get; set; }
}

