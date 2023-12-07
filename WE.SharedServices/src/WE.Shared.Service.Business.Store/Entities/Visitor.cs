using Common.MongoDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WE.Shared.Service.Domain.Entities
{
    public class Visitor : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Nric { get; set; }
        public string Company { get; set; }
        public string Designation { get; set; }
        public Status Status { get; set; } = Status.Pending;
        // image of user
        public string Image { get; set; }
        public string FaceTargetId { get; set; }
        public string FaceTargetKeyId { get; set; }
        public string Type { get; set; } = "Visitor";

    }
    

}
