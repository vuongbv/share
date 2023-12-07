using Common.Models;
using WE.Shared.Service.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WE.SharedService.Application.Models.TenantApplication
{
    public class TenantApplication
    {
        public string DefaultApplication { get; set; }
        public List<string>? Application { get; set; }

    }

    public class TenantApplicationSearch : SearchBaseViewModel
    {

    }
}
