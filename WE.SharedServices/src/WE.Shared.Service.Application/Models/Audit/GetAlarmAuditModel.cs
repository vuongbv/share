using WE.SharedService.Application.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WE.SharedService.Application.Models.Audit;

public class GetAlarmAuditModel
{
    public int? PageIndex { get; set; } = 1;
    public int? PageSize { get; set; } = int.MaxValue;
    public AlarmAuditStatus? Status { get; set; }
    public AlarmAuditType? AuditType { get; set; }
    public string? DeviceId { get; set; }
    public string? DeviceName { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public string? SortBy { get; set; }
    public AuditSortOrder? SortOrder { get; set; }
}
