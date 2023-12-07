using Common.MongoDb.Entities;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WE.Shared.Service.Domain.Entities;

public class WorkFlow: BaseEntity
{
    public string Type { get; set; }
    public BsonValue Value { get; set; }
}

public static class WorkFlowConstants
{

}