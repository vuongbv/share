using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WE.SharedService.Application.Models.Tenant;

public class GenerateApiKeyModel
{
    public string TenantId { get; set; }
    public DateTimeOffset ExpiryTime { get; set; } = default!;
    public List<string> Scopes { get; set; }
}

