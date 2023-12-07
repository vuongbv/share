using Common.MongoDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WE.Shared.Service.Domain.Entities
{
    public class ApplicationName : BaseEntity
    {
        public string Name { get; set; }
    }
}
