using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WE.SharedService.Application.Models.Users;

public class CreateUserModel
{
    //public string UserName { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Email { get; set; } = default!;
    public IList<Common.Identity.Users.Role> Roles { get; set; } = default!;

    public UserSource? UserSource { get; set; }
}

public enum UserSource
{
    AzureAdB2c
}
