using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace WE.SharedService.Application.Models.Widgets;

public class CreateWidgetModelValidator : AbstractValidator<CreateWidgetModel>
{
    public CreateWidgetModelValidator()
    {
    }
}

public class CreateWidgetModel : WidgetBaseModel
{
    public IFormFile? WidgetIcon { get; set; }
}

