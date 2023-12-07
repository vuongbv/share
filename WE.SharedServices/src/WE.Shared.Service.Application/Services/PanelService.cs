using AutoMapper;
using Common.Mongodb.Collections;
using FluentValidation;
using WE.Shared.Service.Domain.Entities;
using WE.Shared.Service.Domain.Repository;
using WE.SharedService.Application.Dtos;
using WE.SharedService.Application.Models.Panel;

namespace WE.SharedService.Application.Services;
public interface IPanelService
{
    public Task<PanelDto> CreatePanel(CreatePanelModel request);
    public Task<bool> UpdatePanel(string id, UpdatePanelModel request);
    public Task<bool> DeletePanel(string PanelId);
    public Task<PagedList<PanelDto>> GetAllPanel(SearchPanelViewModel search);
    public Task<List<PanelDto>> GetAllPanel();
    public Task<PanelDto> GetPanel(string id);
}

public class PanelService : IPanelService
{
    private readonly IPanelRepository _PanelStore;
    private readonly IWidgetRepository _managementRepository;
    private readonly IMapper _mapper;

    public PanelService(IPanelRepository PanelStore, IMapper _mapper, IWidgetRepository managementRepository)
    {
        this._PanelStore = PanelStore;
        this._mapper = _mapper;
        _managementRepository = managementRepository;
    }

    public async Task<List<PanelDto>> GetAllPanel()
    {
        var listResult = _PanelStore.GetAll().ToList();
        var mapped =  _mapper.Map<List<PanelDto>>(listResult);
        return mapped;
    }

    public async Task<PanelDto> CreatePanel(CreatePanelModel request)
    {
        var validator = await (new CreatePanelModelValidator()).ValidateAsync(request);

        if (!validator.IsValid)
        {
            throw new ValidationException(validator.Errors);
        }

        var Panel = _mapper.Map<Panel>(request);

        var result = _PanelStore.Insert(Panel);

        var response = _mapper.Map<PanelDto>(result);
        return response;
    }

    public async Task<PagedList<PanelDto>> GetAllPanel(SearchPanelViewModel search)
    {
        var listResult = await _PanelStore.GetAll().ToPagedList(search);
        var result = _mapper.Map<PagedList<PanelDto>>(listResult);
        return result;
    }

    public async Task<bool> UpdatePanel(string id, UpdatePanelModel model)
    {
        var validator = await (new UpdatePanelModelValidator()).ValidateAsync(model);

        if (!validator.IsValid)
        {
            throw new ValidationException(validator.Errors);
        }

        var entity = _PanelStore.GetFirstOrDefault(x => x.Id == id);

        if (entity != null)
        {
            var Panel = _mapper.Map(model, entity);
            Panel.UpdatedDate = DateTime.UtcNow;
            _PanelStore.Update(Panel, x => x.Id == Panel.Id);
            return true;
        }

        return false;
    }

    public async Task<bool> DeletePanel(string id)
    {
        await _PanelStore.DeleteAsync(x => x.Id == id);
        return true;
    }
    public async Task<PanelDto> GetPanel(string id)
    {
        var Panel = _PanelStore.GetFirstOrDefault(x => x.Id == id);
        var widget =await _managementRepository.FindAsync(x => x.PanelId == id);
        var result = _mapper.Map<PanelDto>(Panel);
        result.Widgets = _mapper.Map<IEnumerable<WidgetDto>>(widget);
        return result;
    }
}

