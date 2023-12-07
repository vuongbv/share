using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WE.SharedService.Application.Models.Keyword;
using WE.Shared.Service.Domain.Entities;

namespace WE.SharedService.Application.Dtos
{
    public class KeywordDto
    {
        public string Id { get; set; }
        public Keyword Keys{get;set;}
    }
}
