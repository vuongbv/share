using AutoMapper;
using Common.Extensions;
using Common.MongoDb.Audits;
using Common.Validate;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WE.SharedService.Application.Models.PowerBI;
using WE.SharedService.Application.Services;
using WE.Shared.Service.Domain.Repository;

namespace WE.Shared.Application;

public static class SharedServiceExtensions
{

    public static void AddSharedApplicationService(this IServiceCollection services)
    {
        services.AddSharedRepository();
        services.AddSharedService();
    }

    private static void AddSharedRepository(this IServiceCollection services)
    {
        services.AddScoped<ITenantStoreExtended, TenantStoreExtented>();
        services.AddScoped<IAlarmRepository, AlarmRepository>();
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IDevicesRepository, DevicesRepository>();
        services.AddScoped<IPanelRepository, PanelRepository>();
        services.AddScoped<IWidgetRepository, WidgetRepository>();
        services.AddScoped<IAuditRepository, AuditRepository>();
        services.AddScoped<ISettingRepository, SettingRepository>();
        services.AddScoped<ITenantSettingRepository, TenantSettingRepository>();
        services.AddScoped<IApplicationNameRepository, ApplicationNameRepository>();
        services.AddScoped<IAVAuthenticationFlowRepository, AVAuthenticationFlowRepository>();
        services.AddScoped<IVisitorRepository, VisitorRepository>();
    }

    private static void AddSharedService(this IServiceCollection services)
    {
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<IAlarmService, AlarmService>();
        services.AddScoped<IGroupService, GroupService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IDeviceStatusService, DeviceStatusService>();
        services.AddScoped<IPanelService, PanelService>();
        services.AddScoped<IWidgetService, WidgetService>();
        services.AddScoped<ISettingService, SettingService>();
        services.AddScoped<IApplicationNameService, ApplicationNameService>();
        services.AddScoped<IAVAuthenticationFlowService, AVAuthenticationFlowService>();

    }

    public static void AddPowerBIService(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IPowerBIAuthenticationService, PowerBIAuthenticationService>();

        var AuthorityUri = config.GetRequiredValue("AzureAdForPowerBI:AuthorityUri");
        var DirectoryId = config.GetRequiredValue("AzureAdForPowerBI:DirectoryId");
        var TenantId = config.GetRequiredValue("AzureAdForPowerBI:TenantId");
        var GrantType = config.GetRequiredValue("AzureAdForPowerBI:GrantType");
        var Resource = config.GetRequiredValue("AzureAdForPowerBI:Resource");
        var ClientId = config.GetRequiredValue("AzureAdForPowerBI:ClientId");
        var ClientSecret = config.GetRequiredValue("AzureAdForPowerBI:ClientSecret");

        var powerBIAzureAdB2cOptions = new AzureAdForPowerBIOptions()
        {
            AuthorityUri = AuthorityUri,
            DirectoryId = DirectoryId,
            TenantId = TenantId,
            GrantType = GrantType,
            Resource = Resource,
            ClientId = ClientId,
            ClientSecret = ClientSecret,
        };

        services.AddSingleton(powerBIAzureAdB2cOptions);
    }

}

