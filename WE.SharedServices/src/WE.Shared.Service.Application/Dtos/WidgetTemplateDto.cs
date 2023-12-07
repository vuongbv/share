using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WE.Shared.Service.Domain.Entities;

namespace WE.SharedService.Application.Dtos
{
    public class WidgetTemplateDto
    {
        public string Id { get; set; }
        public WidgetTemplate Template{get;set;}
    }
}
