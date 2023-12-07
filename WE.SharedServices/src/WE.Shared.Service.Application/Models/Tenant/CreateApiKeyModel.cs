using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WE.SharedService.Application.Models.Tenant;

public class CreateApiKeyModel
{
    public DateTimeOffset ExpiryTime { get; set; }
    public List<string>? Scopes { get; set; }
}
