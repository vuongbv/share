using Common.MongoDb.Audits;
using Common.MongoDb.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using WE.SharedService.Application.Constants;
using WE.SharedService.Application.Dtos;
using WE.SharedService.Application.Models.Audit;
using System.Text.Json;

namespace WE.SharedService.Application.Services;

public interface IAuditService
{
    public Task CreateLastUserLogin(CreateUserLastLoginModel model);
    public Task<MongoPagedList<AlarmAuditDto>> GetAlarmAudit(GetAlarmAuditModel model);
    public Task<List<RoleAuditDto>> GetRoleAudit(GetRoleAuditModel model);
    public Task<MongoPagedList<LastLoginAuditDto>> GetLastLoginAudit(GetLastLoginModel model);

}

public class AuditService : IAuditService
{
    private readonly IAuditRepository _auditRepository;
    private readonly IDeviceStatusService _deviceStatusService;

    public AuditService(IAuditRepository auditRepository, IDeviceStatusService deviceStatusService)
    {
        this._auditRepository = auditRepository ?? throw new ArgumentNullException(nameof(auditRepository));
        this._deviceStatusService = deviceStatusService ?? throw new ArgumentNullException(nameof(deviceStatusService));
    }


    public async Task CreateLastUserLogin(CreateUserLastLoginModel model)
    {
        var audit = new Audit()
        {
            BeforeValue = BsonNull.Value,
            AfterValue = model != null ? BsonDocument.Parse(JsonSerializer.Serialize(model)) : BsonNull.Value,
            CollectionName = "User",
            Type = AuditConstants.UserLastLogin,
            CreatedDate = DateTime.UtcNow
        };

        await _auditRepository.InsertAsync(audit);
    }

    private string GetAuditType(AlarmAuditType? type)
    {
        if (type == null) return null;
        switch (type)
        {
            case AlarmAuditType.Create:
                {

                    return "Create_Alarm";
                }
            case AlarmAuditType.Update:
                {

                    return "Update_Alarm";
                }
            case AlarmAuditType.Delete:
                {

                    return "Delete_Alarm";
                }
            default:
                {
                    return null;
                }
        }
    }

    private string GetAuditAlarmStatus(AlarmAuditStatus? status)
    {
        if (status == null) return null;
        switch (status)
        {
            case AlarmAuditStatus.Acknowledged:
                {
                    return AlarmConstant.Acknowledged;
                }
            case AlarmAuditStatus.Active:
                {
                    return AlarmConstant.Active;
                }
            case AlarmAuditStatus.Resolved:
                {
                    return AlarmConstant.Resolved;
                }
            default:
                return null;
        }
    }

    public async Task<MongoPagedList<AlarmAuditDto>> GetAlarmAudit(GetAlarmAuditModel model)
    {
        var auditType = GetAuditType(model.AuditType);

        var alarmStatus = GetAuditAlarmStatus(model.Status);

        var listActiveDevice = await _deviceStatusService.GetActiveDevice(new List<string>());

        var filterDocList = new List<BsonDocument>();

        var typeMatch = new BsonDocument("$match",
                            new BsonDocument
                                {
                                    { "CollectionName", "Alarm" },
                                    { "CreatedDate",
                            new BsonDocument
                                    {
                                        { "$gt",
                            new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, model.StartDate.Hour, model.StartDate.Minute, model.StartDate.Second) },
                                        { "$lt",
                            new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, model.EndDate.Hour, model.EndDate.Minute, model.EndDate.Second) }
                                    } }
                                });

        filterDocList.Add(typeMatch);

        if(model.DeviceId != null)
        {
            var match = new BsonDocument("$match",
                    new BsonDocument("AfterValue.Source.Id", model.DeviceId));

            filterDocList.Add(match);
        }

        if (model.DeviceName != null)
        {
            var match = new BsonDocument("$match",
                    new BsonDocument("AfterValue.Source.Name",model.DeviceName));

            filterDocList.Add(match);
        }

        if (model.AuditType != null)
        {
            var match = new BsonDocument("$match",
                    new BsonDocument("Type", model.AuditType.Value.toAlarmTypeString()));

            filterDocList.Add(match);
        }

        if (model.Status != null)
        {
            var match = new BsonDocument("$match",
                    new BsonDocument("AfterValue.Status", model.Status.Value.toAlarmStatusString()));

            filterDocList.Add(match);
        }

        var projection = new BsonDocument("$project",
            new BsonDocument
        {
            { "TimeUpdated", "$CreatedDate" },
            { "DeviceId", "$AfterValue.Source.Id" },
            { "DeviceName", "$AfterValue.Source.Name" },
            { "PreiousStatus", "$BeforeValue.Status" },
            { "NewStatus", "$AfterValue.Status" },
            { "AlarmMessage", "$AfterValue.Text" },
            { "Type", "$Type" },
            { "CollectionName", "$CollectionName" }
        });

        filterDocList.Add(projection);

        var facet = new BsonDocument("$facet",
    new BsonDocument
        {
            { "items",
    new BsonArray
            {
                new BsonDocument("$skip", model.PageSize * (model.PageIndex-1)),
                new BsonDocument("$limit", model.PageSize)
            } },
            { "totalCount",
    new BsonArray
            {
                new BsonDocument("$count", "count")
            } }
        });
        
        var pagedList = 
    new BsonDocument("$project",
    new BsonDocument
        {
            { "Items", "$items" },
            { "TotalPage",
    new BsonDocument("$ceil",
    new BsonDocument("$divide",
    new BsonArray
                    {
                        new BsonDocument("$arrayElemAt",
                        new BsonArray
                            {
                                "$totalCount.count",
                                0
                            }),
                        model.PageSize
                    })) }
        });

        filterDocList.Add(facet);

        filterDocList.Add(pagedList);

        PipelineDefinition<Audit, MongoPagedList<AlarmAuditDto>> pipeline = filterDocList.ToArray();


        try
        {
            var data = (await _auditRepository.Collection.AggregateAsync<MongoPagedList<AlarmAuditDto>>(pipeline)).ToList().FirstOrDefault();


            if (!string.IsNullOrEmpty(model.SortBy))
            {
                switch (model.SortBy)
                {
                    case "DeviceName":
                        {
                            data.Items = model.SortOrder == AuditSortOrder.Descending ? data.Items.OrderByDescending(x => x.DeviceName).ToList() : data.Items.OrderBy(x => x.DeviceName).ToList();
                            break;
                        }
                    case "Time":
                        {
                            data.Items = model.SortOrder == AuditSortOrder.Descending ? data.Items.OrderByDescending(x => x.TimeUpdated).ToList() : data.Items.OrderBy(x => x.TimeUpdated).ToList();
                            break;
                        }
                    case "DeviceId":
                        {
                            data.Items = model.SortOrder == AuditSortOrder.Descending ? data.Items.OrderByDescending(x => x.DeviceId).ToList() : data.Items.OrderBy(x => x.TimeUpdated).ToList();
                            break;
                        }
                    case "Status":
                        {
                            data.Items = model.SortOrder == AuditSortOrder.Descending ? data.Items.OrderByDescending(x => x.NewStatus).ToList() : data.Items.OrderBy(x => x.TimeUpdated).ToList();
                            break;
                        }
                    case "AlarmMessage":
                        {
                            data.Items = model.SortOrder == AuditSortOrder.Descending ? data.Items.OrderByDescending(x => x.AlarmMessage).ToList() : data.Items.OrderBy(x => x.TimeUpdated).ToList();
                            break;
                        }
                    default:
                        {

                            break;
                        }
                }
            }

            return data;
        }
        catch (Exception ex)
        {
            return null;
        }

    }

    private bool GetUserIsBlockedStatus(UserAuditStatus? status)
    {
        if (status == null) return false;
        switch (status)
        {
            case UserAuditStatus.Active:
                {
                    return false;
                }
            case UserAuditStatus.Blocked:
                {
                    return true;
                }
            default:
                return false;
        }
    }

    public async Task<List<RoleAuditDto>> GetRoleAudit(GetRoleAuditModel model)
    {
        var userBlockedStatus = GetUserIsBlockedStatus(model.UserStatus);


        var filterDocList = new List<BsonDocument>();

        var audittypeMatch = new BsonDocument("$match",
    new BsonDocument
        {
            { "CollectionName", "User" },
            { "CreatedDate",
   new BsonDocument
                                    {
                                        { "$gt",
                            new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, model.StartDate.Hour, model.StartDate.Minute, model.StartDate.Second) },
                                        { "$lt",
                            new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, model.EndDate.Hour, model.EndDate.Minute, model.EndDate.Second) }
            } }
        });

        filterDocList.Add(audittypeMatch);

        if (model.UserName != null)
        {
            var match = new BsonDocument("$match",
                    new BsonDocument("AfterValue.UserName", model.UserName));

            filterDocList.Add(match);
        }

        if (model.Email != null)
        {
            var match = new BsonDocument("$match",
                    new BsonDocument("AfterValue.Email", model.Email));

            filterDocList.Add(match);
        }

        var projectMatch = new BsonDocument("$project",
    new BsonDocument
        {
            { "UserName", "$BeforeValue.UserName" },
            { "Email", "$BeforeValue.Email" },
            { "BeforeRole", "$BeforeValue.Roles.Name" },
            { "AfterRole", "$AfterValue.Roles.Name" },
            { "DifferenceRole",
    new BsonDocument("$setDifference",
    new BsonArray
                {
                    "$AfterValue.Roles.Name",
                    "$BeforeValue.Roles.Name"
                }) },
            { "TimeUpdated", "$CreatedDate" },
            { "IsBlocked", "$AfterValue.IsBlocked" }
        });

            var arrSizeMatch = 
    new BsonDocument("$match",
    new BsonDocument("$expr",
    new BsonDocument("$gt",
    new BsonArray
                {
                    new BsonDocument("$size", "$DifferenceRole"),
                    0
                })));


        filterDocList.Add(projectMatch);
        filterDocList.Add(arrSizeMatch);

        PipelineDefinition<Audit, RoleAuditDto> pipeline = filterDocList.ToArray();

        try
        {
            var data = (await _auditRepository.Collection.AggregateAsync<RoleAuditDto>(pipeline)).ToList();

            if (!string.IsNullOrEmpty(model.SortBy))
            {
                switch (model.SortBy)
                {
                    case "UserName":
                        {
                            data = model.SortOrder == AuditSortOrder.Descending ? data.OrderByDescending(x => x.UserName).ToList() : data.OrderBy(x => x.UserName).ToList();
                            break;
                        }
                    case "Email":
                        {
                            data = model.SortOrder == AuditSortOrder.Descending ? data.OrderByDescending(x => x.Email).ToList() : data.OrderBy(x => x.Email).ToList();
                            break;
                        }
                    case "TimeUpdated":
                        {
                            data = model.SortOrder == AuditSortOrder.Descending ? data.OrderByDescending(x => x.TimeUpdated).ToList() : data.OrderBy(x => x.TimeUpdated).ToList();
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }

            return data;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<MongoPagedList<LastLoginAuditDto>> GetLastLoginAudit(GetLastLoginModel model)
    {
        var filterDocList = new List<BsonDocument>();

        var audittypeMatch = new BsonDocument("$match",
    new BsonDocument
        {
            { "CollectionName", "User" },
            { "CreatedDate",
   new BsonDocument
                                    {
                                        { "$gt",
                            new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, model.StartDate.Hour, model.StartDate.Minute, model.StartDate.Second) },
                                        { "$lt",
                            new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, model.EndDate.Hour, model.EndDate.Minute, model.EndDate.Second) }
            } }
        });

        filterDocList.Add(audittypeMatch);

        if (model.UserName != null)
        {
            var match = new BsonDocument("$match",
                    new BsonDocument("AfterValue.UserInfo.user_name", model.UserName));

            filterDocList.Add(match);
        }

        if (model.Email != null)
        {
            var match = new BsonDocument("$match",
                    new BsonDocument("AfterValue.UserInfo.email", model.Email));

            filterDocList.Add(match);
        }



        var projection = new BsonDocument("$project",
    new BsonDocument
        {
            { "UserName", "$AfterValue.UserInfo.user_name" },
            { "Email", "$AfterValue.UserInfo.email" },
            { "LoginTime", "$CreatedDate" }
        });

        filterDocList.Add(projection);

        var faceMAtch = new BsonDocument("$facet",
    new BsonDocument
        {
            { "items",
    new BsonArray
            {
                new BsonDocument("$skip", model.PageSize * (model.PageIndex-1)),
                new BsonDocument("$limit", model.PageSize)
            } },
            { "totalCount",
    new BsonArray
            {
                new BsonDocument("$count", "count")
            } }
        });

        var pagedListProject = new BsonDocument("$project",
    new BsonDocument
        {
            { "Items", "$items" },
            { "TotalPage",
    new BsonDocument("$ceil",
    new BsonDocument("$divide",
    new BsonArray
                    {
                        new BsonDocument("$arrayElemAt",
                        new BsonArray
                            {
                                "$totalCount.count",
                                0
                            }),
                        model.PageSize
                    })) }
        });

        filterDocList.Add(faceMAtch);
        filterDocList.Add(pagedListProject);

        PipelineDefinition<Audit, MongoPagedList<LastLoginAuditDto>> pipeline = filterDocList.ToArray();


        try
        {
            var data = (await _auditRepository.Collection.AggregateAsync<MongoPagedList<LastLoginAuditDto>>(pipeline)).ToList().FirstOrDefault();

            if (!string.IsNullOrEmpty(model.SortBy))
            {
                switch (model.SortBy)
                {
                    case "UserName":
                        {
                            data.Items = model.SortOrder == AuditSortOrder.Descending ? data.Items.OrderByDescending(x => x.UserName).ToList() : data.Items.OrderBy(x => x.UserName).ToList();
                            break;
                        }
                    case "Email":
                        {
                            data.Items = model.SortOrder == AuditSortOrder.Descending ? data.Items.OrderByDescending(x => x.Email).ToList() : data.Items.OrderBy(x => x.Email).ToList();
                            break;
                        }
                    case "LoginTime":
                        {
                            data.Items = model.SortOrder == AuditSortOrder.Descending ? data.Items.OrderByDescending(x => x.LoginTime).ToList() : data.Items.OrderBy(x => x.LoginTime).ToList();
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }

            return data;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

}

