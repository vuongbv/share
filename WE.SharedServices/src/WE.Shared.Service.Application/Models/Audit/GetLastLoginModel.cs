using WE.SharedService.Application.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WE.SharedService.Application.Models.Audit;

public class GetLastLoginModel
{
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public int? PageIndex { get; set; } = 1;
    public int? PageSize { get; set; } = int.MaxValue;
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public string? SortBy { get; set; }
    public AuditSortOrder? SortOrder { get; set; }
}

