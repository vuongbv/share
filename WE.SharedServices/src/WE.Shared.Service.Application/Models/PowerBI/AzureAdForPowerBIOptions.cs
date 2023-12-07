using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WE.SharedService.Application.Models.PowerBI;

public class AzureAdForPowerBIOptions
{
    public string AuthorityUri { get; set; } = default!;
    public string DirectoryId { get; set; } = default!;
    public string TenantId { get; set; } = default!;
    public string GrantType { get; set; } = default!;
    public string Resource { get; set; } = default!;
    public string ClientId { get; set; } = default!;
    public string ClientSecret { get; set; } = default!;
}

