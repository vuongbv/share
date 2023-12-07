using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WE.SharedService.Application.Models.Alarm
{
    public class UpdateAlarmModelValidator : AbstractValidator<UpdateAlarmModel>
    {
        public UpdateAlarmModelValidator()
        {
        }
    }

    public class UpdateAlarmModel: AlarmBaseModel
    {
    }
}
