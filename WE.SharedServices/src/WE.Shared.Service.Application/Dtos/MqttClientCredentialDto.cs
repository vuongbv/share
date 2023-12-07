using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WE.SharedService.Application.Dtos;

public class MqttClientCredentialDto
{
    public string ClientId { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}
