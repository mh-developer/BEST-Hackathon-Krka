using Hackathon.Domain.Shared;
using System;

namespace Hackathon.Domain.UserAggregate
{
    public interface IUserRepository : IRepository<User, Guid>
    {
    }
}