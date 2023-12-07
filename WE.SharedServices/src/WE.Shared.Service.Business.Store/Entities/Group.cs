using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.MongoDb.Entities;

namespace WE.Shared.Service.Domain.Entities
{
    public class Group:BaseEntity
    {
        public string Name { get; set; }
        public Location Location { get; set; }
        public bool IsHomePage { get; set; }
        public IEnumerable<string> DeviceIds { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public string AppId { get; set; }
    }

    public class Location
    {
        public int Latitude { get; set; }
        public int Longitude { get; set; }
    }

    public enum AppName
    {
        SolarPowerManagement,
        SmartWaterMonitoring,
        AirQualityMonitoring,
        SurveillanceSecurity,
        SmartCCTVMonitoring,
        SmartRetailMonitoring,
        PowerMonitoringManagement
    }
}
