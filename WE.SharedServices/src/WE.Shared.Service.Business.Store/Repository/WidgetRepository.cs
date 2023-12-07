using Common.Mongodb;
using Common.Mongodb.MultiTenant;
using Common.Mongodb.Repository;
using Common.MongoDbMultitenant;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using WE.Shared.Service.Domain.Entities;

namespace WE.Shared.Service.Domain.Repository;

public interface IWidgetRepository: IRepository<Widget>
{

}


public class WidgetRepository: Repository<Widget>, IWidgetRepository
{
    public WidgetRepository(IMultiTenantService<Tenant> tenantService, IMongoClient client) : base(tenantService, client)
    {

    }

    static WidgetRepository()
    {

        BsonClassMap.RegisterClassMap<Widget>(cm =>
        {
            cm.AutoMap();
        });
    }


    protected override string CollectionName()
    {
        return "Widget";
    }

    protected override Task SetupCollectionAsync(IMongoCollection<Widget> collection,
        CancellationToken ct)
    {
        //setup index
        return Task.CompletedTask;
    }
}

