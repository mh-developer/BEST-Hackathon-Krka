using Hackathon.Application.Shared;
using Hackathon.Domain.WarehouseAggregate;
using System;

namespace Hackathon.Application.DaliveryPoints
{
    public class DeliveryPointDto : EntityDto<Guid>
    {
        public string Name { get; set; }

        public Warehouse Warehouse { get; set; }

        public Guid? WarehouseId { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }
    }
}