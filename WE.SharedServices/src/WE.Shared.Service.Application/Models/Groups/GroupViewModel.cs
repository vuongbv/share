using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WE.Shared.Service.Domain.Entities;

namespace WE.SharedService.Application.Models.Groups
{
    public class GroupViewModel
    {
        public string Name { get; set; }
        public Location Location { get; set; }
        public bool IsHomePage { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public string AppId { get; set; }
    }
}
