using Common.Exceptions;
using Common.Mongodb.MultiTenant;
using Common.MongoDbMultitenant;
using Common.MongoDbMultitenant.TenantIdentification;
using WE.Shared.Service.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WE.SharedService.Application.Services
{
    public class CustomMultiTenantService : IMultiTenantService<Tenant>
    {
        private readonly IEnumerable<ITenantIdentificationStrategy> _tenantIdentificationStrategy;
        private readonly ITenantStoreExtended _tenantStoreExtended;

        public CustomMultiTenantService(ITenantStoreExtended tenantStoreExtended, IEnumerable<ITenantIdentificationStrategy> tenantIdentificationStrategy)
        {
            _tenantStoreExtended = tenantStoreExtended ?? throw new ArgumentNullException(nameof(tenantStoreExtended));
            _tenantIdentificationStrategy = tenantIdentificationStrategy ?? throw new ArgumentNullException(nameof(tenantIdentificationStrategy));
        }

        public async Task<Tenant> GetTenantAsync()
        {

            foreach (var identifierStratery in _tenantIdentificationStrategy)
            {
                string name = identifierStratery.GetType().Name;

                switch (name)
                {
                    case "RequestHostTenantIdentificationStrategy":
                        {
                            var subdomain = await identifierStratery.GetTenantIdentifierAsync();

                            if (string.IsNullOrEmpty(subdomain))
                            {
                                continue;
                            }

                            var tenant = (await _tenantStoreExtended.FindAsync(x => x.SubDomain == subdomain)).FirstOrDefault();
                            if (tenant != null) return tenant;
                            continue;
                        }
                    case "RequestParamTenantIdentificationStraTery":
                        {
                            var tenantName = await identifierStratery.GetTenantIdentifierAsync();

                            if (string.IsNullOrEmpty(tenantName))
                            {
                                continue;
                            }

                            // the tenanat name should be unique, if not maybe cause exception
                            var tenant = (await _tenantStoreExtended.FindAsync(x => x.Name == tenantName)).FirstOrDefault();
                            
                            if (tenant != null) return tenant;
                            continue;
                        }
                    default:
                        break;
                }
                
            }
            return null;
        }


    }
}
