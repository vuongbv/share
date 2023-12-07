using Common.Mongodb;
using Common.Mongodb.MultiTenant;
using Common.Mongodb.Repository;
using Common.MongoDbMultitenant;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using WE.Shared.Service.Domain.Entities;

namespace WE.Shared.Service.Domain.Repository;

public interface IApplicationNameRepository : IRepository<ApplicationName>
{

}


public class ApplicationNameRepository: Repository<ApplicationName>, IApplicationNameRepository
{
    public ApplicationNameRepository(IMongoDatabase database, IMongoClient client)
            : base(database, client)
    {

    }

    static ApplicationNameRepository()
    {

        BsonClassMap.RegisterClassMap<ApplicationName>(cm =>
        {
            cm.AutoMap();
        });
    }


    protected override string CollectionName()
    {
        return "ApplicationName";
    }

    protected override Task SetupCollectionAsync(IMongoCollection<ApplicationName> collection,
        CancellationToken ct)
    {
        //setup index
        return Task.CompletedTask;
    }
}

