using Common.Mongodb;
using Common.Mongodb.MultiTenant;
using Common.Mongodb.Repository;
using Common.MongoDbMultitenant;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using WE.Shared.Service.Domain.Entities;

namespace WE.Shared.Service.Domain.Repository;

public interface ITenantSettingRepository : IRepository<TenantSetting>
{

}
public class TenantSettingRepository : Repository<TenantSetting>, ITenantSettingRepository
{


    public TenantSettingRepository(IMultiTenantService<Common.Mongodb.MultiTenant.Tenant> tenantService, IMongoClient client) : base(tenantService, client)
    {

    }

    static TenantSettingRepository()
    {
        BsonClassMap.RegisterClassMap<TenantSetting>(cm =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);
        });
    }

    protected override string CollectionName()
    {
        return "TenantSetting";
    }

    protected override Task SetupCollectionAsync(IMongoCollection<TenantSetting> collection,
        CancellationToken ct)
    {
        //setup index
        return Task.CompletedTask;
    }
}


