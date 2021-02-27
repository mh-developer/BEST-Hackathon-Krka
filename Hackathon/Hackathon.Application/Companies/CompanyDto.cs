using Hackathon.Application.Shared;
using System;

namespace Hackathon.Application.Companies
{
    public class CompanyDto : EntityDto<Guid>
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }
    }
}