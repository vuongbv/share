using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WE.Shared.Service.Domain.Entities;

namespace WE.SharedService.Application.Dtos
{
    public class GroupDto
    {
        public string  Id { get; set; }
        public string  Name { get; set; }
        public bool IsHomePage { get; set; }
        public IEnumerable<string>  DevicesName { get; set; }
        public IEnumerable<string> DeviceIds { get; set; }
        public IEnumerable<DeviceDto> Devices { get; set; }
        public Location Location { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public string AppId { get; set; }
        public string AppName { get; set; }
    }

    public class DeviceDto
    {
        public string Id { get; set; }
        public string  Name { get; set; }
    }
}
