using Common.Mongodb;
using Common.Mongodb.MultiTenant;
using Common.Mongodb.Repository;
using Common.MongoDbMultitenant;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using WE.Shared.Service.Domain.Entities;

namespace WE.Shared.Service.Domain.Repository;

public interface IGroupRepository: IRepository<Group>
{

}


public class GroupRepository: Repository<Group>,IGroupRepository
{
    public GroupRepository(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
    {

    }

    static GroupRepository()
    {

        BsonClassMap.RegisterClassMap<Group>(cm =>
        {
            cm.AutoMap();
        });
    }


    protected override string CollectionName()
    {
        return "Group";
    }

    protected override Task SetupCollectionAsync(IMongoCollection<Group> collection,
        CancellationToken ct)
    {
        //setup index
        return Task.CompletedTask;
    }
}

