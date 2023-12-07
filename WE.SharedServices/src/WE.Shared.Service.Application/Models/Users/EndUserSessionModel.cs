using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WE.SharedService.Application.Models.Users;

public class EndUserSessionModel
{
    public string ObjectId { get; set; } = default!;
    public UserSource UserSource { get; set; }
}
