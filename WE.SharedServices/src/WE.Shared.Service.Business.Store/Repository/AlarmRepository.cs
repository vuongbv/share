using Common.Mongodb;
using Common.Mongodb.MultiTenant;
using Common.Mongodb.Repository;
using Common.MongoDbMultitenant;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using WE.Shared.Service.Domain.Entities;

namespace WE.Shared.Service.Domain.Repository;

public interface IAlarmRepository: IRepository<Alarm>
{

}


public class AlarmRepository: Repository<Alarm>,IAlarmRepository
{
    public AlarmRepository(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
    {

    }

    static AlarmRepository()
    {

        BsonClassMap.RegisterClassMap<Alarm>(cm =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);
        });
    }


    protected override string CollectionName()
    {
        return "Alarm";
    }

    protected override Task SetupCollectionAsync(IMongoCollection<Alarm> collection,
        CancellationToken ct)
    {
        //setup index
        return Task.CompletedTask;
    }
}

