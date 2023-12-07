using AutoMapper;
using Common.Identity.Users;
using Common.Mongodb.Collections;
using Common.Mongodb.MultiTenant;
using Common.MongoDb;
using MongoDB.Bson;
using WE.SharedService.Application.Dtos;
using WE.SharedService.Application.Models.Alarm;
using WE.SharedService.Application.Models.Tenant;
using WE.Shared.Service.Domain.Entities;
using WE.SharedService.Application.Models.Groups;
using WE.SharedService.Application.Models.Panel;
using WE.SharedService.Application.Models.Users;
using WE.SharedService.Application.Models.Widgets;
using WE.SharedService.Application.Models.WidgetTemplate;
using WE.SharedService.Application.Models.AVAuthenticationFlow;

namespace WE.Shared.Application.Common;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        //Tenant mapper
      
        CreateMap<TenantUpdateViewModel, Tenant>()
            .ForMember(i=>i.SubDomain,d=>d.MapFrom((src, dest) => string.IsNullOrEmpty(dest.SubDomain) ? src.Name.ToLower().Trim().Replace(" ", "") : dest.SubDomain))
            .ForMember(i=>i.Database,d=>d.MapFrom((src, dest) => string.IsNullOrEmpty(dest.Database) ? src.Name.ToLower().Trim().Replace(" ", "") : dest.Database))
            .ForMember(i=>i.UpdatedDate,d=>d.MapFrom(i=>DateTime.UtcNow));
        CreateMap<Tenant, TenantDto>().ForMember(i=>i.URL,d=>d.MapFrom(f=>f.SubDomain+".WE.com"));

        CreateMap<PagedList<Tenant>, PagedList<TenantDto>>();
        CreateMap<CreateAlarmModel, Alarm>();
        CreateMap<UpdateAlarmModel, Alarm>();
        CreateMap<Alarm, AlarmDto>();
        CreateMap<DeviceAlarm, DeviceAlarmModel>();
        CreateMap<DeviceAlarmModel, DeviceAlarm>();
        CreateMap<PagedList<Alarm>, PagedList<AlarmDto>>();

        CreateMap<Device, DeviceDto>().ForMember(i=>i.Id,d=>d.MapFrom(n=>n.SourceId));
        CreateMap<Group, GroupDto>();
        CreateMap<PagedList<Group>, PagedList<GroupDto>>();
        CreateMap<GroupViewModel, Group>();

        CreateMap<User, UserDto>();
        CreateMap<UserViewModel, User>();
        CreateMap<PagedList<User>, PagedList<UserDto>>();

        CreateMap<Widget, WidgetDto>();
        CreateMap<PagedList<Widget>, PagedList<WidgetDto>>();
        CreateMap<CreateWidgetModel, Widget>();
        CreateMap<UpdateWidgetModel,Widget>();
        CreateMap<Panel, PanelDto>();
        CreateMap<PagedList<Panel>, PagedList<PanelDto>>();
        CreateMap<CreatePanelModel, Panel>();
        CreateMap<UpdatePanelModel,Panel>();
        CreateMap<ApiKey, ApiKeyDto>().ForMember(i => i.Key, d => d.MapFrom(n => n.HasedKey));
        CreateMap<PagedList<ApiKey>, PagedList<ApiKeyDto>>();
        CreateMap<PagedList<ApplicationName>, PagedList<ApplicationName>>();
        CreateMap<PagedList<AVAuthenticationFlow>, PagedList<AVAuthenticationFlowDto>>();
        CreateMap<CreateAVAuthenticationFlowModel, AVAuthenticationFlow>();
        CreateMap<UpdateAVAuthenticationFlowModel, AVAuthenticationFlow>();
        CreateMap<AVAuthenticationFlow, AVAuthenticationFlowDto>();

        CreateMap<RegisterWorkerModel, AVAuthenticationFlow>();

        CreateMap<VisitorModel, Visitor>();
    }
}
