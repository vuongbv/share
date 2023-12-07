using Common.Mongodb;
using Common.Mongodb.MultiTenant;
using Common.Mongodb.Repository;
using Common.MongoDbMultitenant;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using WE.Shared.Service.Domain.Entities;

namespace WE.Shared.Service.Domain.Repository
{
    public interface IDevicesRepository : IRepository<Device>
    {

    }


    public class DevicesRepository : Repository<Device>, IDevicesRepository
    {
        public DevicesRepository(IMultiTenantService<Tenant> tenantService, IMongoClient client) : base(tenantService, client)
        {

        }

        static DevicesRepository()
        {

            BsonClassMap.RegisterClassMap<Device>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);

            });
        }


        protected override string CollectionName()
        {
            return "Device";
        }

        protected override Task SetupCollectionAsync(IMongoCollection<Device> collection,
            CancellationToken ct)
        {
            //setup index
            return Task.CompletedTask;
        }
    }

}
