using Common.MongoDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WE.Shared.Service.Domain.Entities
{
    public class AVAuthenticationFlow : BaseEntity
    {
        public DateTime WorkTo { get; set; }
        public DateTime WorkFrom { get; set; }
        public string Purpose { get; set; }
        public Status Status { get; set; } = Status.Pending;
        public string? Contractor { get; set; }
        public List<string>? ListOfPersonnel { get; set; }
        public List<RequestLocation> Locations { get; set; }
        public string? DetailOfWorks { get; set; }
        public string? OTP { get; set; }
        public DateTime? OTPExpireDate { get; set; }
        public string Type { get; set; } = "Session";
    }

    public enum Status
    {
        Pending, 
        Approved,
        Rejected
    }

    public class AVAuthenticationListingDto : BaseEntity
    {
        public DateTime WorkTo { get; set; }
        public DateTime WorkFrom { get; set; }
        public string? Purpose { get; set; }
        public List<RequestLocation>? RequestLocations { get; set; }
        public Status Status { get; set; }
        public List<string> NameOfContractors { get; set; }
    }

}
