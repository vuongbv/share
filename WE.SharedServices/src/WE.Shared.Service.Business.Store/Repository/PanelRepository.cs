using Common.Mongodb;
using Common.Mongodb.MultiTenant;
using Common.Mongodb.Repository;
using Common.MongoDbMultitenant;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using WE.Shared.Service.Domain.Entities;

namespace WE.Shared.Service.Domain.Repository;

public interface IPanelRepository: IRepository<Panel>
{

}


public class PanelRepository: Repository<Panel>,IPanelRepository
{
    public PanelRepository(IMultiTenantService<Tenant> tenantService, IMongoClient client) : base(tenantService, client)
    {

    }

    static PanelRepository()
    {

        BsonClassMap.RegisterClassMap<Panel>(cm =>
        {
            cm.AutoMap();
        });
    }


    protected override string CollectionName()
    {
        return "Panel";
    }

    protected override Task SetupCollectionAsync(IMongoCollection<Panel> collection,
        CancellationToken ct)
    {
        //setup index
        return Task.CompletedTask;
    }
}

