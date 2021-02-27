using Hackathon.Domain.CompanyAggregate;
using Hackathon.Domain.Shared;
using System;

namespace Hackathon.Domain.DeliveryAggregate
{
    public class Delivery : Entity<Guid>
    {
        public int Code { get; set; }

        public DeliveryPoint DeliveryPoint { get; set; }

        public Guid? DeliveryPointId { get; set; }

        public DeliveryStatus Status { get; set; }

        public DateTime? DispatchTime { get; set; }

        public DateTime? DeliveryTime { get; set; }

        public DateTime? CreationTime { get; set; }

        public Company SourceCompany { get; set; }

        public Guid? SourceCompanyId { get; set; }

        public Company DestinationCompany { get; set; }

        public Guid? DestinationCompanyId { get; set; }
    }

    public enum DeliveryStatus
    {
        None = 0,
        InProgress = 1,
        Rejected = 2,
        Received = 3
    }
}