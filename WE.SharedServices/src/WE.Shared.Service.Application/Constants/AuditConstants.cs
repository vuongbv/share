using NodaTime.TimeZones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WE.SharedService.Application.Constants;

public static class AuditConstants
{
    public const string UserLastLogin = "User_Last_Login";
    public const string Acknowledged = "acknowledged";
    public const string Resolved = "resolved";
    public const string Active = "active";
}

public enum AuditSortOrder
{
    Descending,
    Ascending,
}

public static class AuditExtension
{
    public static string toAlarmStatusString(this AlarmAuditStatus status)
    {
        switch(status)
        {
            case Constants.AlarmAuditStatus.Active:
                {
                    return "active";
                }
            case Constants.AlarmAuditStatus.Resolved:
                {
                    return "resolved";
                }
            case Constants.AlarmAuditStatus.Acknowledged:
                {
                    return "acknowledged";
                }
            default:
                return string.Empty;
        }
    }
    public static string toAlarmTypeString(this AlarmAuditType status)
    {
        switch (status)
        {
            case Constants.AlarmAuditType.Create:
                {
                    return "Create_Alarm";
                }
            case Constants.AlarmAuditType.Update:
                {
                    return "Update_Alarm";
                }
            case Constants.AlarmAuditType.Delete:
                {
                    return "Delete_Alarm";
                }
            default:
                return string.Empty;
        }
    }
}

public enum AlarmAuditStatus
{
    Acknowledged,
    Resolved,
    Active
}

public enum AlarmAuditType
{
    Create,
    Update,
    Delete
}

public enum UserAuditStatus
{
    Blocked,
    Active
}