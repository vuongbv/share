using FluentValidation;
using WE.SharedService.Application.Dtos;
using WE.Shared.Service.Domain.Entities;

namespace WE.SharedService.Application.Models.Widgets;

public class UpdateWidgetPositionModelValidator : AbstractValidator<UpdateWidgetPositionModel>
{
    public UpdateWidgetPositionModelValidator()
    {
        RuleFor(x => x.WidgetId).NotEmpty();
    }

}

public class UpdateWidgetPositionModel
{
    public string WidgetId { get; set; }
    public bool? IsHidden { get; set; }
    public WidgetPosition Position { get; set; }
    public WidgetSetting? Setting { get; set; }
}

