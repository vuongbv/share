using Common.MongodbMultitenant;
using System.ComponentModel.DataAnnotations;

namespace WE.SharedService.Application.Models.Tenant;

public class TenantViewModel :TenantUpdateViewModel
{
   
    public string URL { get; set; } = default!;
    public TenantSetting? Setting { get; set; }
   
}

