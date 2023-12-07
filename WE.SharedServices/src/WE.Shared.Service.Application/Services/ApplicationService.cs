using AutoMapper;
using Common.Mongodb.Collections;
using FluentValidation;
using WE.SharedService.Application.Models.ApplicationName;
using WE.Shared.Service.Domain.Entities;
using WE.Shared.Service.Domain.Repository;

namespace WE.SharedService.Application.Services;

public interface IApplicationNameService
{
    public Task<ApplicationName> CreateApplicationName(string? name);
    public Task<bool> UpdateApplicationName(string id,string? name);
    public Task<bool> DeleteApplicationName(string id);
    public Task<PagedList<ApplicationName>> GetAllApplicationName(SearchApplicationNameViewModel search);
    public ApplicationName GetApplicationName(string id);
}

public class ApplicationNameService : IApplicationNameService
{
    private readonly IApplicationNameRepository _applicationStore;
    private readonly IMapper _mapper;

    public ApplicationNameService(IApplicationNameRepository applicationStore, IMapper _mapper)
    {
        this._applicationStore = applicationStore;
        this._mapper = _mapper;
    }

    public async Task<ApplicationName> CreateApplicationName(string name)
    {

        var application = new ApplicationName { Name = name };
        var result = _applicationStore.Insert(application);
        return result;
    }

    public async Task<PagedList<ApplicationName>> GetAllApplicationName(SearchApplicationNameViewModel search)
    {
        var listResult = await _applicationStore.GetAll().ToPagedList(search);
        var result = _mapper.Map<PagedList<ApplicationName>>(listResult);
        return result;
    }

    public async Task<bool> UpdateApplicationName(string id, string name)
    {
        

        var entity = _applicationStore.GetFirstOrDefault(x => x.Id == id);

        if(entity != null)
        {
            entity.UpdatedDate = DateTime.UtcNow;
            entity.Name = name;
            _applicationStore.Update(entity, x => x.Id == entity.Id);
            return true;
        }
        
        return false;
    }
    public async Task<bool> DeleteApplicationName(string id)
    {
        await _applicationStore.DeleteAsync(x => x.Id == id);
        return true;
    }
    public ApplicationName GetApplicationName(string id)
    {
        var applicationName = _applicationStore.GetFirstOrDefault(x => x.Id == id);
        return applicationName;
    }
}
