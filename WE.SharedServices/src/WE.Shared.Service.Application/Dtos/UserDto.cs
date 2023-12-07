using Common.Identity.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace WE.SharedService.Application.Dtos
{
    public class UserDto
    {
        public string Id { get; set; }
        public string TenantId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string TenantName { get; set; }
        public string TenantIconFavicon { get; set; }
        public string TenantTitle { get; set; }
        public IList<Role> Roles { get; set; } = default!;
    }
}
