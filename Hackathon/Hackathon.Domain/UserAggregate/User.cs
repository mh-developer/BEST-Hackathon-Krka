using Hackathon.Domain.CompanyAggregate;
using Hackathon.Domain.Shared;
using Hackathon.Domain.WarehouseAggregate;
using Microsoft.AspNetCore.Identity;
using System;

namespace Hackathon.Domain.UserAggregate
{
    public class User : IdentityUser<Guid>, IEntity<Guid>, IHasDeletionTime
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletionTime { get; set; }

        public Company Company { get; set; }

        public Guid? CompanyId { get; set; }

        public Warehouse Warehouse { get; set; }

        public Guid? WarehouseId { get; set; }
    }
}