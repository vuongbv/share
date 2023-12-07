using Common.MongoDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WE.Shared.Service.Domain.Entities
{
    public class RequestLocation
    {
        public string Name { get; set; }
        public decimal? lat { get; set; }
        public decimal? lon { get; set; }
    }

    
}
