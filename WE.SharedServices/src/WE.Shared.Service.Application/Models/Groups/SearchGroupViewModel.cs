using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Models;

namespace WE.SharedService.Application.Models.Groups
{
    public class SearchGroupViewModel:SearchBaseViewModel
    {
        public string? Name { get; set; }
        public string? ApplicationName { get; set; }
    }
}
