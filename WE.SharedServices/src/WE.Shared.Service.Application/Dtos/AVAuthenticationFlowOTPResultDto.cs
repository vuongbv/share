using WE.SharedService.Application.Models.Alarm;
using WE.Shared.Service.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WE.SharedService.Application.Models.AVAuthenticationFlow;

namespace WE.SharedService.Application.Dtos
{
    public class AVAuthenticationFlowOTPResultDto
    {
        public bool Success { get; set; }
        public string OTPState { get; set; }
    }
}
