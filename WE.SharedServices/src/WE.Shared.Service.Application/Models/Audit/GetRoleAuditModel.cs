using WE.SharedService.Application.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WE.SharedService.Application.Models.Audit;

public class GetRoleAuditModel
{
    public int? PageIndex { get; set; }
    public int? PageSize { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public UserAuditStatus? UserStatus { get; set; }
    public string? SortBy { get; set; }
    public AuditSortOrder? SortOrder { get; set; }
}

