using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WE.SharedService.Application.Models.Tenant;

public class CreateMqttClientCredentialModel
{
    public string UserName { get; set;}
    public string Password { get; set;}
}
