using Hackathon.Domain.Shared;
using Hackathon.Domain.WarehouseAggregate;
using System;

namespace Hackathon.Domain.DeliveryAggregate
{
    public class DeliveryPoint : Entity<Guid>
    {
        public string Name { get; set; }

        public Warehouse Warehouse { get; set; }

        public Guid? WarehouseId { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }
    }
}