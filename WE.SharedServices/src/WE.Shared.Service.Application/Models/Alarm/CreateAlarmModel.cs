using FluentValidation;
using WE.Shared.Service.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WE.SharedService.Application.Models.Alarm
{
    public class CreateAlarmModelValidator: AbstractValidator<CreateAlarmModel>
    {
        public CreateAlarmModelValidator()
        {
        }
    }

    public class CreateAlarmModel: AlarmBaseModel
    {

    }
}
