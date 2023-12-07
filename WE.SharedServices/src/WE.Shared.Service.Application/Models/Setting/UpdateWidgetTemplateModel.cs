using FluentValidation;

namespace WE.SharedService.Application.Models.WidgetTemplate;

public class UpdateWidgetTemplateModelValidator : AbstractValidator<UpdateWidgetTemplateModel>
{
    public UpdateWidgetTemplateModelValidator()
    {
        RuleFor(x => x.TemplateName).NotEmpty();
    }

}

public class UpdateWidgetTemplateModel : WidgetTemplateBaseModel
{
}

