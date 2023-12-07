using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using WE.SharedService.Application.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WE.SharedService.Application.Dtos;

public class RoleAuditDto
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string UserStatus { get; set; }
    public DateTimeOffset TimeUpdated { get; set; }
    public List<string> BeforeRole { get; set; }
    public List<string> AfterRole { get; set; }
    public List<string> DifferenceRole { get; set; }
    public bool IsBlocked { get; set; }
}
