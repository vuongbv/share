using WE.Shared.Service.Domain.Entities;

namespace WE.SharedService.Application.Models.AVAuthenticationFlow;

public class AVAuthenticationFlowBaseModel
{
    public DateTime WorkTo { get; set; }
    public DateTime WorkFrom { get; set; }
    public string Purpose { get; set; }

    //public bool IsApproved { get; set; } = false;
    public string Contractor { get; set; }
    public List<string> ListOfPersonnel { get; set; }
    public List<RequestLocation>? Locations { get; set; }
    public string DetailOfWorks { get; set; }
    public string Type { get; set; } = "Session";

}





