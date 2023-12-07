using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WE.SharedService.Application.Models.AVAuthenticationFlow;

public class RegisterWorkerModel: AVAuthenticationFlowBaseModel
{
    public string FaceUrl { get; set; }
}
