using FluentValidation;

namespace WE.SharedService.Application.Models.Panel;

public class CreatePanelModelValidator : AbstractValidator<CreatePanelModel>
{
    public CreatePanelModelValidator()
    {
        RuleFor(x => x.GroupId).NotEmpty();
    }
}

public class CreatePanelModel : PanelBaseModel
{
}

