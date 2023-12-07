using WE.SharedService.Application.Models.Alarm;
using WE.Shared.Service.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WE.SharedService.Application.Models.AVAuthenticationFlow;
using Microsoft.Extensions.Hosting;

namespace WE.SharedService.Application.Dtos
{
    public class AVAuthenticationFlowDto : AVAuthenticationFlowBaseModel
    {
        public string Id { get; set; }
    }

    public class RequestRegisterDto
    {
        public string Company { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public List<RequestLocation> Locations { get; set; }
        public string Email { get; set; }
        public DateTime WorkTo { get; set; }
        public DateTime WorkFrom { get; set; }
        public string NumberOfWorker { get; set; }
    }
}
