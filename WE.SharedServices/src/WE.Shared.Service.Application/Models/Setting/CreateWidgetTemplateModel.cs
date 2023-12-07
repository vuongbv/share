using FluentValidation;

namespace WE.SharedService.Application.Models.WidgetTemplate;

public class CreateWidgetTemplateModelValidator : AbstractValidator<CreateWidgetTemplateModel>
{
    public CreateWidgetTemplateModelValidator()
    {
        RuleFor(x => x.TemplateName).NotEmpty();
    }
}

public class CreateWidgetTemplateModel : WidgetTemplateBaseModel
{
}

