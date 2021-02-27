using Hackathon.Domain.Shared;
using System;

namespace Hackathon.Domain.CompanyAggregate
{
    public interface ICompanyRepository : IRepository<Company, Guid>
    {
    }
}