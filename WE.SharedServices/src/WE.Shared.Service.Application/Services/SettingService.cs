using AutoMapper;
using Common.Mongodb.Collections;
using FluentValidation;
using WE.Shared.Service.Domain.Entities;
using WE.Shared.Service.Domain.Repository;
using WE.SharedService.Application.Dtos;
using WE.SharedService.Application.Models;
using WE.SharedService.Application.Constants;
using MongoDB.Bson;

namespace WE.SharedService.Application.Services;

public interface ISettingService
{
    public Task<Setting> CreateSetting(Setting request);
    public Task<bool> UpdateSetting(string id, Setting request);
    public Task<bool> DeleteSetting(string id);
    public Task<List<Setting>> GetSettingByType(string? settingType);
    public Task<Setting> GetSetting(string id);
}

public class SettingService : ISettingService
{
    private readonly ISettingRepository _settingStore;
    private readonly IMapper _mapper;

    public SettingService(ISettingRepository settingStore, IMapper _mapper)
    {
        this._settingStore = settingStore;
        this._mapper = _mapper;
    }

    public async Task<Setting> CreateSetting(Setting request)
    {

        var result = _settingStore.Insert(request);
        return result;
    }

    public async Task<List<Setting>> GetSettingByType(string? settingType)
    {
        return (await _settingStore.FindAsync(x => x.Key == settingType)).ToList();
    }

    public async Task<bool> UpdateSetting(string id, Setting model)
    {
        var existed = (await _settingStore.FindAsync(x => x.Id == id)).FirstOrDefault();
        if (existed != null)
        {
            existed.UpdatedDate = DateTime.UtcNow;
            existed.Value = model.Value;
            await _settingStore.UpdateAsync(existed, x => x.Id == existed.Id);
            return true;
        }
        return false;
    }
    public async Task<bool> DeleteSetting(string id)
    {
        await _settingStore.DeleteAsync(x => x.Id == id);
        return true;
    }
    public async Task<Setting> GetSetting(string id)
    {
        return (await _settingStore.FindAsync(x => x.Id == id)).FirstOrDefault();
    }
}
