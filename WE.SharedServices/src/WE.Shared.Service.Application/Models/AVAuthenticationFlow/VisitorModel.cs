using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WE.SharedService.Application.Models.AVAuthenticationFlow;

public class VisitorModel
{
    
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Nric { get; set; }
    public string Company { get; set; }
    public string Designation { get; set; }
    public string? PhoneNumber { get; set; }
    // image of user
    public string? Image { get; set; }
    public string Type { get; set; } = "Visitor";
}
