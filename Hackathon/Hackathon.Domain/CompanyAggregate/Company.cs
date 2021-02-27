using Hackathon.Domain.Shared;
using System;

namespace Hackathon.Domain.CompanyAggregate
{
    public class Company : Entity<Guid>
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }
    }
}