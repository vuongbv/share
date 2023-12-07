using Common.Identity.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WE.SharedService.Application.Models.Users
{
    public class UserViewModel
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public IList<Role> Roles { get; set; } = default!;
    }
}
