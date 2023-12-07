using Common.MongodbMultitenant;
using WE.SharedService.Application.Models.Tenant;

namespace WE.SharedService.Application.Dtos;

public class TenantDto:TenantViewModel
{
    public string Id { get; set; }
  
}