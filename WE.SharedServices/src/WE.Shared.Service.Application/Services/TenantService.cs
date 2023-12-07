using System.Data;
using AutoMapper;
using Common.Exceptions;
using Common.Mongodb.Collections;
using WE.Shared.Service.Domain.Repository;
using Common.Services;
using StackExchange.Redis.Extensions.Core.Abstractions;
using WE.SharedService.Application.Dtos;
using WE.SharedService.Application.Models.Tenant;
using Common.Identity.Authentication.GTKeys;
using Common.Mongodb.MultiTenant;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace WE.SharedService.Application.Services
{
    public interface ITenantService
    {
        Task<PagedList<TenantDto>> Seach(SearchTenantViewModel search);
        Task<string> Create(TenantViewModel model);
        Task Update(string id, TenantUpdateViewModel model);
        Task<TenantDto> GetById(string id);
        Task Delete(string id);
        Task<string> GenerateApiKey(GenerateApiKeyModel apiKeyModel);
        Task<PagedList<ApiKeyDto>> SearchGenerateApiKey(SearchApiKeyViewModel search);
        Task<ApiKeyDto> GetGenerateApiKey(string id);
        Task DeleteGenerateApiKey(string id);
        public Task<MqttClientCredentialDto> GenerateMqttClient(GenerateMqttClientCredentialModel model);


    }

    public class TenantService : ITenantService
    {
        private readonly ITenantStoreExtended _tenantStoreExtended;
        private readonly IMapper _mapper;
        private readonly IRedisClient _redisClient;
        private readonly IApiKeyService _apiKeyService;
        private readonly ApiKeySetting _apiKeySetting;

        public TenantService(ITenantStoreExtended tenantStoreExtended,
            IMapper mapper,
            IRedisClient redisClient,
            IApiKeyService apiKeyService,
            ApiKeySetting apiKeySetting)
        {
            _tenantStoreExtended = tenantStoreExtended;
            _mapper = mapper;
            _redisClient = redisClient;
            _apiKeyService = apiKeyService ?? throw new ArgumentNullException(nameof(apiKeyService));
            _apiKeySetting = apiKeySetting ?? throw new ArgumentNullException(nameof(apiKeySetting));
        }

        public async Task<PagedList<TenantDto>> Seach(SearchTenantViewModel search)
        {
            var query = _tenantStoreExtended.GetAll();


            if (!string.IsNullOrEmpty(search.Name))
            {
                query = query.Where(x => x.Name == search.Name);
            }

            return _mapper.Map<PagedList<TenantDto>>(await query.ToPagedList(search));
        }

        public async Task<string> Create(TenantViewModel model)
        {
            var check = await _tenantStoreExtended.FindAsync(i =>
                i.Name.Equals(model.Name.Trim().ToLower()));

            if (check.Any()) throw new DuplicateNameException();
            var tenant = _mapper.Map<Common.Mongodb.MultiTenant.Tenant>(model);
            var result = await _tenantStoreExtended.InsertAsync(tenant);
            return result.Id;
        }

        public async Task Update(string id, TenantUpdateViewModel model)
        {
            var modelUpdate = _tenantStoreExtended.GetFirstOrDefault(i => i.Id.Equals(id));
            _mapper.Map(model, modelUpdate);
            await _tenantStoreExtended.UpdateAsync(modelUpdate, x => x.Id == id);
        }

        public async Task<TenantDto> GetById(string id)
        {
            return _mapper.Map<TenantDto>(_tenantStoreExtended.GetFirstOrDefault(i => i.Id.Equals(id)));
        }

        public async Task Delete(string id)
        {
            await _tenantStoreExtended.DeleteAsync(x => x.Id == id);
        }

        public async Task<string> GenerateApiKey(GenerateApiKeyModel apiKeyModel)
        {
            var key = _apiKeyService.GenerateApiKey();

            var hashed = _apiKeyService.GenerateHash(key, _apiKeySetting.Salt);

            var tenant = (await _tenantStoreExtended.FindAsync(x => x.Id == apiKeyModel.TenantId)).FirstOrDefault();

            var apiKey = new ApiKey()
            {
                HasedKey = hashed,
                ExpiryTime = apiKeyModel.ExpiryTime,
                Scopes = apiKeyModel.Scopes ?? _apiKeySetting.DefaultScopes
            };

            var currentApiKey = new List<ApiKey>();

            var apiKeyDto = new ApiKeyDto()
            {
                Key = key,
                ExpiryTime = apiKeyModel.ExpiryTime,
                Scopes = apiKey.Scopes ?? _apiKeySetting.DefaultScopes
            };

            if (tenant.ApiKeys != null && tenant.ApiKeys.Any())
            {
                currentApiKey = tenant.ApiKeys;
                currentApiKey.Add(apiKey);
                tenant.ApiKeys = currentApiKey;

                await _tenantStoreExtended.UpdateAsync(tenant, x => x.Id == tenant.Id);
                return apiKeyDto.Key;

            }

            currentApiKey.Add(apiKey);

            tenant.ApiKeys = currentApiKey;

            await _tenantStoreExtended.UpdateAsync(tenant, x => x.Id == tenant.Id);

            return apiKeyDto.Key;
        }

        public async Task<PagedList<ApiKeyDto>> SearchGenerateApiKey(SearchApiKeyViewModel search)
        {
            var keys = await _tenantStoreExtended.GetAll().SelectMany(i => i.ApiKeys).ToPagedList(search);
            return _mapper.Map<PagedList<ApiKeyDto>>(keys);
        }

        public async Task<ApiKeyDto> GetGenerateApiKey(string id)
        {
            var tenant = _tenantStoreExtended.GetFirstOrDefault(i => i.ApiKeys.Any(n => n.HasedKey == id));
            if (tenant == null)
            {
                throw new NotFoundException();
            }

            var result = _mapper.Map<ApiKeyDto>(tenant.ApiKeys.FirstOrDefault(i => i.HasedKey == id));
            result.TenantId = tenant.Id;

            return result;
        }

        public async Task DeleteGenerateApiKey(string id)
        {
            var tenant = _tenantStoreExtended.GetFirstOrDefault(i => i.ApiKeys.Any(n => n.HasedKey == id));
            if (tenant == null)
            {
                throw new NotFoundException();
            }

            tenant.ApiKeys = tenant.ApiKeys.Where(i => i.HasedKey != id).ToList();
            await _tenantStoreExtended.UpdateAsync(tenant,i=>i.Id == tenant.Id);

        }

        public async Task<MqttClientCredentialDto> GenerateMqttClient(GenerateMqttClientCredentialModel model)
        {
            var clientId = _apiKeyService.GenerateApiKey();

            var mqttClient = new MqttClient()
            {
                UserName = model.UserName,
                HashedPassword = _apiKeyService.GenerateHash(model.Password, ""),
                ClientId = clientId
            };

            var mqttClientDto = new MqttClientCredentialDto
            {
                UserName = model.UserName,
                Password = model.Password,
                ClientId = clientId
            };

            var tenant = (await _tenantStoreExtended.FindAsync(x => x.Id == model.TenantId)).FirstOrDefault();

            var currentMqttClients = new List<MqttClient>();

            if (tenant.MqttClients != null && tenant.MqttClients.Any())
            {
                currentMqttClients = tenant.MqttClients;

                currentMqttClients.Add(mqttClient);
                tenant.MqttClients = currentMqttClients;

                await _tenantStoreExtended.UpdateAsync(tenant, x => x.Id == tenant.Id);

                return mqttClientDto;
            }

            currentMqttClients.Add(mqttClient);
            tenant.MqttClients = currentMqttClients;

            await _tenantStoreExtended.UpdateAsync(tenant, x => x.Id == tenant.Id);

            return mqttClientDto;
        }
    }
}
