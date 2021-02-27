using Hackathon.Application.Shared;
using Hackathon.Domain.CompanyAggregate;
using System;

namespace Hackathon.Application.Warehouses
{
    public class WarehouseDto : EntityDto<Guid>
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