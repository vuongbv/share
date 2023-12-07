using Common.Mongodb;
using Common.Mongodb.MultiTenant;
using Common.Mongodb.Repository;
using Common.MongoDbMultitenant;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using WE.Shared.Service.Domain.Entities;

namespace WE.Shared.Service.Domain.Repository;

public interface ISettingRepository: IRepository<Setting>
{

}


public class SettingRepository: Repository<Setting>, ISettingRepository
{
    public SettingRepository(IMongoDatabase database, IMongoClient client)
            : base(database, client)
    {

    }

    static SettingRepository()
    {

        BsonClassMap.RegisterClassMap<Setting>(cm =>
        {
            cm.AutoMap();
        });
    }


    protected override string CollectionName()
    {
        return "Setting";
    }

    protected override Task SetupCollectionAsync(IMongoCollection<Setting> collection,
        CancellationToken ct)
    {
        //setup index
        return Task.CompletedTask;
    }
}

