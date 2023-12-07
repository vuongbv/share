using AutoMapper;
using Common.Mongodb.Collections;
using FluentValidation;
using WE.Shared.Service.Domain.Entities;
using WE.Shared.Service.Domain.Repository;
using WE.SharedService.Application.Dtos;
using WE.SharedService.Application.Models.Widgets;

namespace WE.SharedService.Application.Services;

public interface IWidgetService
{
    public Task<WidgetDto> CreateWidget(CreateWidgetModel request);
    public Task<bool> UpdateWidget(string id, UpdateWidgetModel request);
    public Task<bool> DeleteWidget(string widgetId);
    public Task<PagedList<WidgetDto>> GetAllWidget(SearchWidgetViewModel search);
    public WidgetDto GetWidget(string id);
    public Task<bool> UpdateWidgetPosition(List<UpdateWidgetPositionModel> models);
}

public class WidgetService : IWidgetService
{
    private readonly IWidgetRepository _widgetStore;
    private readonly IMapper _mapper;

    public WidgetService(IWidgetRepository widgetStore, IMapper _mapper)
    {
        this._widgetStore = widgetStore;
        this._mapper = _mapper;
    }

    public async Task<WidgetDto> CreateWidget(CreateWidgetModel request)
    {
        var validator = await (new CreateWidgetModelValidator()).ValidateAsync(request);

        if (!validator.IsValid)
        {
            throw new ValidationException(validator.Errors);
        }

        var Widget = _mapper.Map<Widget>(request);
        var result = _widgetStore.Insert(Widget);
        var response = _mapper.Map<WidgetDto>(result);
        return response;
    }

    public async Task<PagedList<WidgetDto>> GetAllWidget (SearchWidgetViewModel search)
    {
        var listResult = await _widgetStore.GetAll().ToPagedList(search);
        var result = _mapper.Map<PagedList<WidgetDto>>(listResult);
        return result;
    }

    public async Task<bool> UpdateWidget(string id, UpdateWidgetModel model)
    {
        var validator = await (new UpdateWidgetModelValidator()).ValidateAsync(model);

        if (!validator.IsValid)
        {
            throw new ValidationException(validator.Errors);
        }

        var entity = _widgetStore.GetFirstOrDefault(x => x.Id == id);

        if(entity != null)
        {
            var Widget = _mapper.Map(model, entity);
            Widget.UpdatedDate = DateTime.UtcNow;
            _widgetStore.Update(Widget, x => x.Id == Widget.Id);
            return true;
        }
        
        return false;
    }

    public async Task<bool> UpdateWidgetPosition(List<UpdateWidgetPositionModel> models)
    {
        foreach(var item in models)
        {
            var entity = _widgetStore.GetFirstOrDefault(x => x.Id == item.WidgetId);

            if (entity != null)
            {
                if(item.IsHidden != null)
                {
                    entity.IsHidden = item.IsHidden.Value;
                }
                entity.Position = item.Position;
                entity.UpdatedDate = DateTime.UtcNow;
                _widgetStore.Update(entity, x => x.Id == entity.Id);
            }
        }
        return true;
    }

    public async Task<bool> DeleteWidget(string id)
    {
        await _widgetStore.DeleteAsync(x => x.Id == id);
        return true;
    }
    public WidgetDto GetWidget(string id)
    {
        var Widget = _widgetStore.GetFirstOrDefault(x => x.Id == id);
        return _mapper.Map<WidgetDto>(Widget);
    }
}
