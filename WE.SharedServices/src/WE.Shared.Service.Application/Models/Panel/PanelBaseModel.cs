
using WE.Shared.Service.Domain.Entities;

namespace WE.SharedService.Application.Models.Panel;

public class PanelBaseModel
{
    public string PanelName { get; set; }
    public string GroupId { get; set; }
    public IEnumerable<string> Roles{ get; set; }

    public string? DeviceId { get; set; }

    public List<Tab>? Tabs { get; set; }
}

