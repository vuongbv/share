using Common.Mongodb;
using Common.Mongodb.MultiTenant;
using Common.Mongodb.Repository;
using Common.MongoDbMultitenant;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using WE.Shared.Service.Domain.Entities;

namespace WE.Shared.Service.Domain.Repository;

public interface IVisitorRepository : IRepository<Visitor>
{

}


public class VisitorRepository: Repository<Visitor>, IVisitorRepository
{
    public VisitorRepository(IMongoDatabase database, IMongoClient client)
            : base(database, client)
    {

    }

    static VisitorRepository()
    {

        BsonClassMap.RegisterClassMap<Visitor>(cm =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);
        });
    }


    protected override string CollectionName()
    {
        return "AVAuthenticationFlow";
    }

    protected override Task SetupCollectionAsync(IMongoCollection<Visitor> collection,
        CancellationToken ct)
    {
        //setup index
        return Task.CompletedTask;
    }
}

