using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WE.SharedService.Application.Dtos;

public class ApiKeyDto
{
    public string Key { get; set; }
    public DateTimeOffset ExpiryTime { get; set; }
    public List<string> Scopes { get; set; } = default!;
    public string TenantId { get; set; }
}

