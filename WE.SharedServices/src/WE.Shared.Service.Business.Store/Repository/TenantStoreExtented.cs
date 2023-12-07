using Common.MongodbMultitenant.Store;
using MongoDB.Driver;

namespace WE.Shared.Service.Domain.Repository;


public interface ITenantStoreExtended : ITenantStore
{

}

public class TenantStoreExtented: TenantStore, ITenantStoreExtended
{
    public TenantStoreExtented(IMongoDatabase database, IMongoClient client)
        : base(database, client)
    {
        
    }
    
}