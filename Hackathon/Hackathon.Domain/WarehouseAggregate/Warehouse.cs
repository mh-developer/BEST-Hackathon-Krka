using Hackathon.Domain.CompanyAggregate;
using Hackathon.Domain.Shared;
using System;

namespace Hackathon.Domain.WarehouseAggregate
{
    public class Warehouse : Entity<Guid>
    {
        public int? MinCode { get; set; }

        public int? MaxCode { get; set; }

        public Company Company { get; set; }

        public Guid? CompanyId { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }
    }
}