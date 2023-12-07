using Common.Mongodb;
using Common.Mongodb.MultiTenant;
using Common.Mongodb.Repository;
using Common.MongoDbMultitenant;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using WE.Shared.Service.Domain.Entities;

namespace WE.Shared.Service.Domain.Repository;

public interface IAVAuthenticationFlowRepository : IRepository<AVAuthenticationFlow>
{

}


public class AVAuthenticationFlowRepository: Repository<AVAuthenticationFlow>, IAVAuthenticationFlowRepository
{
    public AVAuthenticationFlowRepository(IMongoDatabase database, IMongoClient client)
            : base(database, client)
    {

    }

    static AVAuthenticationFlowRepository()
    {

        BsonClassMap.RegisterClassMap<AVAuthenticationFlow>(cm =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);
        });
    }


    protected override string CollectionName()
    {
        return "AVAuthenticationFlow";
    }

    protected override Task SetupCollectionAsync(IMongoCollection<AVAuthenticationFlow> collection,
        CancellationToken ct)
    {
        //setup index
        return Task.CompletedTask;
    }
}

