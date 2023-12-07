using FluentValidation;

namespace WE.SharedService.Application.Models.Panel;

public class UpdatePanelModelValidator: AbstractValidator<UpdatePanelModel>
{
    public UpdatePanelModelValidator()
    {
        RuleFor(x => x.GroupId).NotEmpty();
    }

}

public class UpdatePanelModel: PanelBaseModel
{
}

