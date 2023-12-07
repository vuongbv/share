using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace WE.SharedService.Application.Models.Widgets;

public class UpdateWidgetModelValidator : AbstractValidator<UpdateWidgetModel>
{
    public UpdateWidgetModelValidator()
    {
    }

}

public class UpdateWidgetModel : WidgetBaseModel
{
    public IFormFile? WidgetIcon { get; set; }
}

